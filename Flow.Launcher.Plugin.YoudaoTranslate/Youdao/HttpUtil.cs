using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.YoudaoTranslate.Youdao;

/// <summary>
/// copy from https://ai.youdao.com/DOCSIRMA/html/trans/api/wbfy/index.html C# Demo
/// </summary>
static class HttpUtil
{
    private static HttpClient client = new HttpClient();

    private const string DefaultUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36";

    static HttpUtil()
    {
        // need to be added so it would work on a win10 machine
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls
                                                | SecurityProtocolType.Tls11
                                                | SecurityProtocolType.Tls12;

        client.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
        // HttpClient.DefaultProxy = WebProxy;
    }

    public static Task<Stream> PostStreamAsync([NotNull] string url, Dictionary<string, string[]> header,
        Dictionary<string, string[]> param,
        CancellationToken token = default) => PostStreamAsync(new Uri(url), header, param, token);


    public static async Task<Stream> PostStreamAsync(
        [NotNull] Uri url, Dictionary<string, string[]> header, Dictionary<string, string[]> param,
        CancellationToken token = default)
    {
        #region 参数设置

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

        #endregion

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = para;
        var response = client.SendAsync(request, token).Result;

        var suc = response.Content.Headers.TryGetValues("Content-Type", out var contentTypeHeader);
        if (suc && contentTypeHeader != null && !((string[])contentTypeHeader)[0].Contains("application/json"))
        {
            return null;
        }

        return response.Content.ReadAsStreamAsync(token).Result;
    }

    [CanBeNull]
    public static Task<HttpResponseMessage> DoPostAsTask(
        HttpClient client,
        string url, Dictionary<string, string[]> header,
        Dictionary<string, string[]> param)
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

        return client.PostAsync(url, para);
    }


    public static byte[] Post(HttpClient client,
        string url,
        [CanBeNull] Dictionary<string, string[]> header,
        [CanBeNull] Dictionary<string, string[]> param,
        string expectContentType)
    {
        #region 参数设置

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

        #endregion

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = para;
        var response = client.Send(request);

        var suc = response.Content.Headers.TryGetValues("Content-Type", out var contentTypeHeader);

        if (suc && contentTypeHeader != null && !((string[])contentTypeHeader)[0].Contains(expectContentType))
        {
            // Console.WriteLine(res.Content.ReadAsStringAsync().Result);
            return null;
        }

        return response.Content.ReadAsByteArrayAsync().Result;
    }

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