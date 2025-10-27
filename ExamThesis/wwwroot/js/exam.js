(function () {
  function z(n) { return n < 10 ? '0' + n : n; }
  function formatTime(ms) {
    var h = Math.floor(ms / 3600000);
    var m = Math.floor((ms % 3600000) / 60000);
    var s = Math.floor((ms % 60000) / 1000);
    return z(h) + ':' + z(m) + ':' + z(s);
  }

  function getAntiForgeryToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
  }

  function sendProgress(examId, questionId, answerId) {
    try {
      var token = getAntiForgeryToken();
      fetch('/Exam/SaveProgress', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': token
        },
        body: JSON.stringify({ examId: parseInt(examId), questionId: parseInt(questionId), answerId: parseInt(answerId) })
      }).catch(function () { /* ignore network errors */ });
    } catch (_) { /* ignore */ }
  }

  function initSingleChoice() {
    var form = document.getElementById('examForm');
    var examIdEl = form ? form.querySelector('input[name="examId"]') : null;
    var examId = examIdEl ? examIdEl.value : '';

    document.querySelectorAll('.answer-checkbox').forEach(function (cb) {
      cb.addEventListener('change', function () {
        var qid = cb.getAttribute('data-question-id');
        // enforce single choice per question
        document.querySelectorAll('.answer-checkbox[data-question-id="' + qid + '"]').forEach(function (other) {
          if (other !== cb) other.checked = false;
          var parent = other.closest('.form-check');
          if (parent) parent.classList.toggle('active', other.checked);
        });
        // persist selection when checked
        if (cb.checked && examId && qid) {
          sendProgress(examId, qid, cb.value);
        }
      });
    });
  }

  function initTimer() {
    // The view sets window.examEndTime = '@ViewBag.EndTime'
    var endTimeStr = (typeof window !== 'undefined') ? window.examEndTime : null;
    if (!endTimeStr) return;

    var endTime = new Date(new Date().toDateString() + ' ' + endTimeStr);
    var initialTotal = endTime - new Date();
    if (initialTotal < 1) initialTotal = 1;

    var timerEl = document.getElementById('timer');
    var barEl = document.getElementById('timerBar');

    function tick() {
      var remaining = endTime - new Date();
      if (timerEl) timerEl.textContent = formatTime(Math.max(remaining, 0));
      if (barEl) {
        var elapsed = initialTotal - remaining;
        var pct = (elapsed / initialTotal) * 100;
        if (pct < 0) pct = 0; if (pct > 100) pct = 100;
        barEl.style.width = pct + '%';
      }
      if (remaining <= 0) {
        var form = document.getElementById('examForm');
        if (form) form.submit();
        return;
      }
      setTimeout(tick, 1000);
    }

    // initial render
    if (timerEl) timerEl.textContent = formatTime(initialTotal);
    tick();
  }

  function initSubmitConfirm() {
    var form = document.getElementById('examForm');
    if (!form) return;
    form.addEventListener('submit', function (e) {
      var confirmMsg = "Are you sure you want to submit the exam? You won't be able to change your answers.";
      if (!window.confirm(confirmMsg)) {
        e.preventDefault();
        e.stopPropagation();
        return false;
      }
      var btn = form.querySelector('button[type="submit"]');
      if (btn) { btn.disabled = true; btn.innerText = 'Submitting...'; }
    });
  }

  function initBfcacheGuard() {
    window.addEventListener('pageshow', function (e) {
      if (e.persisted) {
        window.location.reload();
      }
    });
  }

  function initAll() {
    initSingleChoice();
    initTimer();
    initSubmitConfirm();
    initBfcacheGuard();
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initAll);
  } else {
    initAll();
  }
})();