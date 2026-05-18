using System.Collections.Generic;

namespace WoadNameGen.Serialization;

internal sealed class MarkovNameModelDto
{
    public int Order { get; set; }

    public List<MarkovTransitionDto> Transitions { get; set; }
        = new List<MarkovTransitionDto>();

    public List<string> TrainingSamples { get; set; }
        = new List<string>();
}