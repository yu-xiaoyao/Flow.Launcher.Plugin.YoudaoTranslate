using System.Collections.Generic;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.YoudaoTranslate.Model;

public record TranslationResultModel
{
    public static readonly string SuccessCode = "0";
    public string Query { set; get; }
    public string From { set; get; }
    public string To { set; get; }

    public bool Success { set; get; }

    [CanBeNull] public List<TranslationItemModel> ResultList { set; get; }

    [CanBeNull] public string ErrorMsg { set; get; }

    [CanBeNull] public string ErrorCode { set; get; }
    [CanBeNull] public Dictionary<string, object> Extra { set; get; }
}

public record TranslationItemModel
{
    public string Src { get; set; }
    public string Dst { get; set; }

    [CanBeNull] public Dictionary<string, object> ItemExtra { set; get; }
}