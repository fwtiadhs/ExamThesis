(function () {
  function z(n) { return n < 10 ? '0' + n : n; }
  function formatTime(ms) {
    var h = Math.floor(ms / 3600000);
    var m = Math.floor((ms % 3600000) / 60000);
    var s = Math.floor((ms % 60000) / 1000);
    return z(h) + ':' + z(m) + ':' + z(s);
  }

  function initSingleChoice() {
    document.querySelectorAll('.answer-checkbox').forEach(function (cb) {
      cb.addEventListener('change', function () {
        var qid = cb.getAttribute('data-question-id');
        document.querySelectorAll('.answer-checkbox[data-question-id="' + qid + '"]').forEach(function (other) {
          if (other !== cb) other.checked = false;
          var parent = other.closest('.form-check');
          if (parent) parent.classList.toggle('active', other.checked);
        });
      });
    });
  }

  function initFinalSubmit() {
    var btn = document.getElementById('finalSubmitBtn');
    if (!btn) return;
    btn.addEventListener('click', function () {
      if (confirm('???????? ???????;')) {
        var form = btn.closest('form');
        if (form) form.submit();
      }
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

  document.addEventListener('DOMContentLoaded', function () {
    initSingleChoice();
    initFinalSubmit();
    initTimer();
  });
})();