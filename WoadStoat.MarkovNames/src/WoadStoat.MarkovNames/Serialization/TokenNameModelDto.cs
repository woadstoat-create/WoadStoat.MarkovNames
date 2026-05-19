using System.Collections.Generic;

namespace WoadStoat.MarkovNames.Serialization;

internal sealed class TokenNameModelDto
{
    public int Order { get; set; }

    public NameTokenizerDto Tokenizer { get; set; } = new NameTokenizerDto();

    public List<TokenTransitionDto> Transitions { get; set; }
        = new List<TokenTransitionDto>();

    public List<string> TrainingSamples { get; set; }
        = new List<string>();
}