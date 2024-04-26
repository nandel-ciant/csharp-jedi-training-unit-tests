using Microsoft.Extensions.Configuration;

namespace SampleOfUnitTests;

public record Jedi
{
    public string Name { get; set; } = string.Empty;
    public bool IsSith { get; set; }
    public int Power { get; set; }
}

public interface IJediMastersRepository
{
    Task<Jedi> RandomAsync();
}

public class JediTraining
{
    private readonly IJediMastersRepository _jediMastersRepository;
    private readonly IConfiguration _configuration;

    public JediTraining(IJediMastersRepository jediMastersRepository, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(jediMastersRepository);
        ArgumentNullException.ThrowIfNull(configuration);
        
        _jediMastersRepository = jediMastersRepository;
        _configuration = configuration;
    }

    public async Task TrainAsync(Jedi padawan)
    {
        // The Jedi training rules are:
        // - No Siths can be trained and should throw exceptions if we try to train one
        // - If "Jedi training is available" we should "find a master and start the training"
        // - The power ups after training are as follows
        //    - "Yoda" increases the power by 5
        //    - "Obi-Wan" increases the power by 3
        //    - Other masters increases the power by 2
        
        if (padawan.IsSith)
        {
            throw new InvalidDataException("No siths can be trained in here");
        }
        
        if (IsJediTrainingAvailable())
        {
            await TrainInternalAsync(padawan);
        }
    }

    protected virtual bool IsJediTrainingAvailable()
    {
        return _configuration.GetValue<bool>("IsJediTrainingAvailable");
    }

    protected virtual async Task TrainInternalAsync(Jedi padawan)
    {
        var master = await _jediMastersRepository.RandomAsync();
        padawan.Power += GetPowerUp(master);
    }

    protected virtual int GetPowerUp(Jedi master)
    {
        return master.Name switch
        {
            "Yoda" => 5,
            "Obi-Wan" => 3,
            _ => 2
        };
    }
}