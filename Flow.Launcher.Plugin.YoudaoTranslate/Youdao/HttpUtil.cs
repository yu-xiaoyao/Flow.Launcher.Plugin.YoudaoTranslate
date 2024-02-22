using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.YoudaoTranslate.Youdao;

/// <summary>
/// copy from https://ai.youdao.com/DOCSIRMA/html/trans/api/wbfy/index.html C# Demo
/// </summary>
static class HttpUtil
{
    [CanBeNull]
    public static byte[] DoPost(
        HttpClient client,
        string url, Dictionary<string, string[]>? header,
        Dictionary<string, string[]>? param,
        string expectContentType)
    {
        var content = new StringBuilder();

        if (param != null)
        {
            var i = 0;
            foreach (var p in param)
            {
                foreach (var v in p.Value)
                {
                    if (i > 0) content.Append('&');

                    content.Append($"{p.Key}={HttpUtility.UrlDecode(v)}");
                    i++;
                }
            }
        }

        var para = new StringContent(content.ToString());
        if (header != null)
        {
            para.Headers.Clear();
            foreach (var h in header)
            {
                foreach (var v in h.Value)
                {
                    para.Headers.Add(h.Key, v);
                }
            }
        }

        // try
        // {
            var res = client.PostAsync(url, para).Result;
            var suc = res.Content.Headers.TryGetValues("Content-Type", out var contentTypeHeader);
            if (suc && contentTypeHeader != null && !((string[])contentTypeHeader)[0].Contains(expectContentType))
            {
                // Console.WriteLine(res.Content.ReadAsStringAsync().Result);
                return null;
            }

            return res.Content.ReadAsByteArrayAsync().Result;
        // }
        // catch (Exception ex)
        // {
        //     // Console.WriteLine("http request error: " + ex);
        //     return null;
        // }
    }

    public static byte[] doGet(string url, Dictionary<String, String[]> header, Dictionary<String, String[]> param,
        string expectContentType)
    {
        try
        {
            StringBuilder content = new StringBuilder();
            content.Append(url);
            using (HttpClient client = new HttpClient())
            {
                if (param != null)
                {
                    content.Append("?");
                    int i = 0;
                    foreach (var p in param)
                    {
                        foreach (var v in p.Value)
                        {
                            if (i > 0)
                            {
                                content.Append("&");
                            }

                            content.AppendFormat("{0}={1}", p.Key, HttpUtility.UrlEncode(v));
                            i++;
                        }
                    }
                }

                if (header != null)
                {
                    client.DefaultRequestHeaders.Clear();
                    foreach (var h in header)
                    {
                        foreach (var v in h.Value)
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, v);
                        }
                    }
                }

                var res = client.GetAsync(content.ToString()).Result;
                IEnumerable<string> contentTypeHeader;
                var suc = res.Content.Headers.TryGetValues("Content-Type", out contentTypeHeader);
                if (suc && !((string[])contentTypeHeader)[0].Contains(expectContentType))
                {
                    Console.WriteLine(res.Content.ReadAsStringAsync().Result);
                    return null;
                }

                return res.Content.ReadAsByteArrayAsync().Result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("http request error: " + ex);
            return null;
        }
    }
}