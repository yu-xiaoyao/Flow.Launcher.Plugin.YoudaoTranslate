using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.YoudaoTranslate;

public class Settings : BaseModel
{
    public Settings()
    {
        Url = "https://openapi.youdao.com/api";
        ApiLimitTime = 300;
    }

    public string Url { get; set; }
    [CanBeNull] public string AppId { get; set; }
    [CanBeNull] public string AppSecret { get; set; }
    public int ApiLimitTime { get; set; }
}