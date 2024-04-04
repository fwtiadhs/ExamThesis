using ExamThesis.Common;
using ExamThesis.Services.Services;
using ExamThesis.Storage.Model;
using FakeItEasy;

namespace ExamThesis.Services.Tests
{

    public class ExamServiceShould
    {
        private IExamService _sut;
        private ExamContext _examContext;
        private IExamCategoryService _categoryService;

        public ExamServiceShould()
        {
            _categoryService = A.Fake<IExamCategoryService>();
            _examContext = A.Fake<ExamContext>();
            _sut = new ExamService(_examContext, _categoryService);
        }

        [Fact]
        public async void CreateExam()
        {
            var model = new CreateExam();

            await _sut.CreateExam(model);

            A.CallTo( () => _categoryService.AddCategoriesForExam(model,A<int>.Ignored)).MustHaveHappened();
            

        }

        [Fact]
        public void Setup()
        { 

        }
    }
}