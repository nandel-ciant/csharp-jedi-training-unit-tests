using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace SampleOfUnitTests.UnitTests;

public class JediTrainingTests
{
    private readonly JediTraining _subject;
    private readonly Mock<JediTraining> _mock;
    private readonly JediTrainingStub _stub;
    
    private readonly Mock<IJediMastersRepository> _jediMastersRepository = new();
    private readonly Mock<IConfiguration> _configuration = new();

    public JediTrainingTests()
    {
        _subject = new JediTraining(_jediMastersRepository.Object, _configuration.Object);
        _mock = new Mock<JediTraining>(_jediMastersRepository.Object, _configuration.Object)
        {
            CallBase = true 
        };
        _stub = new JediTrainingStub(_jediMastersRepository.Object, _configuration.Object);
    }

    [Fact]
    public async Task TrainAsync_JediIsSith_ShouldThrownAnException()
    {
        // Arrange
        var jedi = new Jedi()
        {
            IsSith = true
        };
        
        // act && assert
        await _subject.Invoking(x => x.TrainAsync(jedi))
            .Should()
            .ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task TrainAsync_WhenTrainingIsAvailable_ShouldDoTheTraining()
    {
        // Arrange
        var jedi = new Jedi()
        {
            IsSith = false
        };
        
        _mock.Protected()
            .As<IJediTrainingProtectedMethods>()
            .Setup(x => x.IsJediTrainingAvailable())
            .Returns(true);

        _mock.Protected()
            .As<IJediTrainingProtectedMethods>()
            .Setup(x => x.TrainInternalAsync(jedi))
            .Returns(Task.CompletedTask);
        
        // Act
        await _mock.Object.TrainAsync(jedi);
        
        // Assert
        _mock.Protected()
            .As<IJediTrainingProtectedMethods>()
            .Verify(x => x.TrainInternalAsync(jedi), times: Times.Once());
    }

    [Theory]
    [InlineData("Yoda", 5)]
    [InlineData("Obi-Wan", 3)]
    [InlineData("Fernando", 2)]
    public void GetPowerUp_ShouldMatchExpectedResult(string masterName, int expectedResult)
    {
        // arrange
        var master = new Jedi() { Name = masterName };
        
        // act
        var result = _stub.GetPowerUpPublic(master);
        
        // assert
        result.Should().Be(expectedResult);
    }

    private interface IJediTrainingProtectedMethods
    {
        bool IsJediTrainingAvailable();
        Task TrainInternalAsync(Jedi padawan);
    }

    private class JediTrainingStub : JediTraining
    {
        public JediTrainingStub(IJediMastersRepository jediMastersRepository, IConfiguration configuration) : base(jediMastersRepository, configuration)
        {
        }

        public int GetPowerUpPublic(Jedi padawan)
        {
            return GetPowerUp(padawan);
        }
    }
}