using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.YoudaoTranslate.Youdao;

public record BasicResult
{
    [JsonPropertyName("explains")]
    [CanBeNull]
    public string[] Explains { get; set; }
}

public record YoudaoResultModel
{
    [JsonPropertyName("returnPhrase")] public string[] ReturnPhrase { get; set; }
    [JsonPropertyName("query")] public string Query { get; set; }

    [JsonPropertyName("errorCode")] public string ErrorCode { get; set; }

    [JsonPropertyName("l")] public string SourceTargetLang { get; set; }

    [JsonPropertyName("translation")]
    [CanBeNull]
    public List<string> Translation { get; set; }

    [JsonPropertyName("web")] [CanBeNull] public List<WebItem> Web { set; get; }

    [JsonPropertyName("basic")]
    [CanBeNull]
    public BasicResult Basic { set; get; }

    [JsonPropertyName("tSpeakUrl")] public string TSpeakUrl { get; set; }
    [JsonPropertyName("speakUrl")] public string SpeakUrl { get; set; }
}

public record WebItem
{
    [JsonPropertyName("key")] public string Key { get; set; }

    [JsonPropertyName("value")] public List<string> Value { get; set; }
}