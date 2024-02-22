using System;
using System.Collections.Generic;
using System.Text.Json;
using Flow.Launcher.Plugin.YoudaoTranslate.Youdao;

namespace Flow.Launcher.Plugin.YoudaoTranslate;

public record SubRecord
{
    public List<string> subKey { set; get; }
}

public record TestJsonRecord
{
    public string key { set; get; }
    public SubRecord mk { set; get; }
}

public class Main_Test
{
    public static void TestJson()
    {
        var json = """
                   {
                     "key": "test",
                     "mk": {
                       "subKey": ["TEST"]
                     }
                   }
                   """;
        Console.WriteLine(json);
        var result = JsonSerializer.Deserialize<TestJsonRecord>(json);
        Console.WriteLine(result);
    }

    public static void testMockJson()
    {
        var json = """
                   {
                     "returnPhrase": ["爱你"],
                     "query": "爱你",
                     "errorCode": "0",
                     "l": "zh-CHS2en",
                     "tSpeakUrl": "https://openapi.youdao.com/ttsapi?q=love+you.&langType=en-USA&sign=7EE7DC239962D84EA4CE51E42F13E1FE&salt=1708584498989&voice=4&format=mp3&appKey=41469f91566cb242&ttsVoiceStrict=false&osType=api",
                     "web": [
                       { "value": ["Love You", "Loving You", "dj", "love you"], "key": "爱你" },
                       {
                         "value": ["I love you", "ich liebe dich", "Wuh that I love you"],
                         "key": "我爱你"
                       },
                       {
                         "value": [
                           "I Will Always Love You",
                           "te amare por siempre",
                           "I'll Always Love You",
                           "I Will Always Loving You"
                         ],
                         "key": "我将永远爱你"
                       }
                     ],
                     "requestId": "f8057370-6bf3-4c62-b070-6f37e7247a86",
                     "translation": ["love you."],
                     "mTerminalDict": {
                       "url": "https://m.youdao.com/m/result?lang=zh-CHS&word=%E7%88%B1%E4%BD%A0"
                     },
                     "dict": { "url": "yddict://m.youdao.com/dict?le=eng&q=%E7%88%B1%E4%BD%A0" },
                     "webdict": {
                       "url": "http://mobile.youdao.com/dict?le=eng&q=%E7%88%B1%E4%BD%A0"
                     },
                     "basic": {
                       "explains": ["love you"]
                     },
                     "isWord": true,
                     "speakUrl": "https://openapi.youdao.com/ttsapi?q=%E7%88%B1%E4%BD%A0&langType=zh-CHS&sign=7F694DA11D5DD96E1F83D648D1045CCF&salt=1708584498989&voice=4&format=mp3&appKey=41469f91566cb242&ttsVoiceStrict=false&osType=api"
                   }

                   """;

        var result = JsonSerializer.Deserialize<YoudaoResultModel>(json);
        Console.WriteLine($"ReturnPhrase = {result.ReturnPhrase.Length}");
        foreach (var rp in result.ReturnPhrase)
        {
            Console.WriteLine($"ReturnPhrase Item = {rp}");
        }

        Console.WriteLine($"Basic is null = {result.Basic is null}");
        if (result.Basic is not null)
        {
            if (result.Basic.Explains != null)
                foreach (var explain in result.Basic.Explains)
                {
                    Console.WriteLine($"basic explain = {explain}");
                }
        }

        // Console.WriteLine($"mTerminalDict = {result.mTerminalDict}");
    }


    public static void testTranslate()

    {
        var settings = new Settings
        {
            Url = "https://openapi.youdao.com/api",
            AppId = "xxx",
            AppSecret = "xxx"
        };
        var result = YoudaoTranslation.Translation(settings, "爱你");

        Console.WriteLine(result.Success);
        if (result.Success)
        {
            foreach (var model in result.ResultList)
            {
                Console.WriteLine($"model = {model}");
            }
        }
        else
        {
            Console.WriteLine(result.ErrorMsg);
            Console.WriteLine(result.ErrorCode);
        }
    }
}