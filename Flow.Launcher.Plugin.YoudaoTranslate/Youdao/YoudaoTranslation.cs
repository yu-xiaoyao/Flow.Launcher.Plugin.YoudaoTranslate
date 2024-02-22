using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.YoudaoTranslate.Model;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.YoudaoTranslate.Youdao;

/// <summary>
/// https://ai.youdao.com/DOCSIRMA/html/trans/price/wbfy/index.html
/// </summary>
public class YoudaoTranslation
{
    private static bool HasChineseChar(string str)
    {
        // return ChineseRegexAttribute.(str);
        return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
    }


    public static Task<HttpResponseMessage> TranslationTask(Settings settings, string query,
        [CanBeNull] string from = null,
        [CanBeNull] string to = null)
    {
        if (HasChineseChar(query))
        {
            from = "zh-CHS";
            to = "en";
        }
        else
        {
            from = "auto";
            to = "zh-CHS";
        }

        var paramsMap = CreateRequestParams(query, from, to);
        AuthV3Util.addAuthParams(settings.AppId, settings.AppSecret, paramsMap);
        var header = new Dictionary<string, string[]>
        {
            { "Content-Type", new[] { "application/x-www-form-urlencoded" } }
        };


        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        return HttpUtil.DoPostAsTask(client, settings.Url, header, paramsMap);
    }

    public static TranslationResultModel Translation(Settings settings, string query, [CanBeNull] string from = null,
        [CanBeNull] string to = null)
    {
        if (HasChineseChar(query))
        {
            from = "zh-CHS";
            to = "en";
        }
        else
        {
            from = "auto";
            to = "zh-CHS";
        }

        var paramsMap = CreateRequestParams(query, from, to);
        AuthV3Util.addAuthParams(settings.AppId, settings.AppSecret, paramsMap);
        var header = new Dictionary<string, string[]>
        {
            { "Content-Type", new[] { "application/x-www-form-urlencoded" } }
        };


        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        byte[] result;
        try
        {
            result = HttpUtil.DoPost(client, settings.Url, header, paramsMap, "application/json");
        }
        catch (Exception e)
        {
            return ErrorResult(query, "接口请求超时");
        }

        return ConvertToTranslationResult(result, query);
    }

    public static TranslationResultModel ConvertTaskToTranslationResult(Task<HttpResponseMessage> task, string query)
    {
        var res = task.Result;
        var suc = res.Content.Headers.TryGetValues("Content-Type", out var contentTypeHeader);
        if (suc && contentTypeHeader != null && !((string[])contentTypeHeader)[0].Contains("application/json"))
        {
            return null;
        }

        var result = res.Content.ReadAsByteArrayAsync().Result;
        return ConvertToTranslationResult(result, query);
    }


    public static TranslationResultModel ConvertToTranslationResult(byte[] result, string query)
    {
        if (result == null) return ErrorResult(query, "Response body is null or empty.");

        var resultModel = ParserResult(Encoding.UTF8.GetString(result));
        if (resultModel == null) return ErrorResult(query, "Input Json Format Invalid.");

        if (!"0".Equals(resultModel.ErrorCode))
            return ErrorResult(query, $"Response error, errorCode = {resultModel.ErrorCode}", resultModel.ErrorCode);

        var resultList = new List<TranslationItemModel>();

        var duplicateSet = new HashSet<string>(16, StringComparer.InvariantCultureIgnoreCase);

        if (resultModel.Translation != null)
        {
            foreach (var value in resultModel.Translation)
            {
                if (duplicateSet.Contains(value)) continue;

                resultList.Add(new TranslationItemModel
                {
                    Src = query,
                    Dst = value,
                    ItemExtra = new Dictionary<string, object?>
                    {
                        { "Source", "Translation" },
                        { "tSpeakUrl", resultModel.TSpeakUrl },
                        { "speakUrl", resultModel.SpeakUrl },
                        { "returnPhrase", resultModel.ReturnPhrase }
                    }
                });
                duplicateSet.Add(value);
            }
        }


        if (resultModel.Web != null)
        {
            foreach (var webItem in resultModel.Web)
            {
                if (webItem.Value.Count == 0) continue;
                if (query.Equals(webItem.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var itemValue in webItem.Value)
                    {
                        if (duplicateSet.Contains(itemValue)) continue;
                        resultList.Add(new TranslationItemModel
                        {
                            Src = webItem.Key,
                            Dst = itemValue,
                            ItemExtra = new Dictionary<string, object>
                            {
                                { "Source", "Web" }
                            }
                        });
                        duplicateSet.Add(itemValue);
                    }
                }
            }
        }


        if (resultModel.Basic?.Explains != null)
        {
            foreach (var value in resultModel.Basic.Explains)
            {
                if (duplicateSet.Contains(value)) continue;
                resultList.Add(new TranslationItemModel
                {
                    Src = query,
                    Dst = value,
                    ItemExtra = new Dictionary<string, object>
                    {
                        { "Source", "Explains" }
                    }
                });
                duplicateSet.Add(value);
            }
        }


        return new TranslationResultModel
        {
            ErrorCode = resultModel.ErrorCode,
            Success = true,
            Query = query,
            From = "FROM>>>>",
            To = "TO>>>>",
            ResultList = resultList
        };
    }

    public static TranslationResultModel ErrorResult(string query, string errorMsg, string? errorCode = null)
    {
        return new TranslationResultModel
        {
            Query = query,
            Success = false,
            ErrorMsg = errorMsg,
            ErrorCode = errorCode
        };
    }

    [CanBeNull]
    private static YoudaoResultModel ParserResult(string content)
    {
        // Console.WriteLine($"content = {content}");
        // return JsonSerializer.Deserialize<YoudaoResultModel>(content,
        //     new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });


        // var parserResult = JsonSerializer.Deserialize(content, typeof(YoudaoResultModel),
        //     new JsonSerializerOptions() {  });
        // return parserResult as YoudaoResultModel;

        return JsonSerializer.Deserialize<YoudaoResultModel>(content);
    }

    private static Dictionary<string, string[]> CreateRequestParams(string query, string from, string to)
    {
        return new Dictionary<string, string[]>
        {
            { "q", new[] { query } },
            { "from", new[] { from } },
            { "to", new[] { to } }
        };
    }
}