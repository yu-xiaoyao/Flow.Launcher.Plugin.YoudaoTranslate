using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.YoudaoTranslate.Youdao;

namespace Flow.Launcher.Plugin.YoudaoTranslate
{
    public class YoudaoTranslate : IAsyncPlugin, IContextMenu, ISettingProvider
    {
        private const string IconPath = "Images\\YoudaoTranslate.png";

        private PluginInitContext _context;
        private Settings _settings;

        public Task InitAsync(PluginInitContext context)
        {
            return Task.Run(() => Init(context));
        }

        private void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }


        public Task<List<Result>> QueryAsync(Query query, CancellationToken token)
        {
            // _context.API.LogInfo("YD",
            //     $"QueryAsync. CanBeCanceled: {token.CanBeCanceled}. HashCode: {token.GetHashCode()}");
            // return new Task<List<Result>>(() => Query(query, token));
            return Task.FromResult(Query(query, token));
        }

        private List<Result> Query(Query query, CancellationToken token)
        {
            if (string.IsNullOrEmpty(_settings.AppId) || string.IsNullOrEmpty(_settings.AppSecret))
            {
                return new List<Result>
                {
                    new()
                    {
                        IcoPath = IconPath,
                        Title = "AppId 或者 AppSecret 没有配置",
                        SubTitle = "进入设置中配置或者回车配置",
                        Action = _ =>
                        {
                            ShowSettingPanelDialog();
                            return true;
                        }
                    }
                };
            }

            var search = query.Search.Trim();
            if (string.IsNullOrWhiteSpace(search))
                return null;

            if (_settings.ApiLimitTime > 0)
            {
                // 输入太快, API 查询会限制.
                Thread.Sleep(TimeSpan.FromMilliseconds(_settings.ApiLimitTime));
            }


            var translationResult = YoudaoTranslation.Translation(_settings, search, token);

            if (!translationResult.Success)
            {
                return new List<Result>
                {
                    new()
                    {
                        IcoPath = IconPath,
                        Title = "翻译接口出错",
                        SubTitle = $"异常信息: {translationResult.ErrorMsg}"
                    }
                };
            }

            return translationResult.ResultList?.Select(itemModel => new Result
                {
                    Title = itemModel.Src,
                    SubTitle = itemModel.Dst,
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(itemModel.Dst);
                        return true;
                    }
                })
                .ToList();
        }


        public Control CreateSettingPanel()
        {
            return new SettingsControlPanel(_context, _settings);
        }


        public List<Result> LoadContextMenus(Result selectedResult)
        {
            return new List<Result>();
        }

        private void ShowSettingPanelDialog()
        {
            var window = new SettinsWindow(_context, _settings);
            window.ShowDialog();
        }
    }
}