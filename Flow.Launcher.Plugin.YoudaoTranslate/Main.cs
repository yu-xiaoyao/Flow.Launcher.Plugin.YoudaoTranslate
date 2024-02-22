using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Flow.Launcher.Plugin.YoudaoTranslate.Youdao;

namespace Flow.Launcher.Plugin.YoudaoTranslate
{
    public class YoudaoTranslate : IPlugin, IContextMenu, ISettingProvider
    {
        private const string IconPath = "YoudaoTranslate.png";

        private PluginInitContext _context;
        private Settings _settings;


        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }


        public Task<List<Result>> QueryAsync(Query query, CancellationToken token)
        {
            return Task.FromResult(Query(query));
        }

        public List<Result> Query(Query query)
        {
            if (string.IsNullOrEmpty(_settings.AppId) || string.IsNullOrEmpty(_settings.AppSecret))
            {
                return new List<Result>
                {
                    new()
                    {
                        IcoPath = IconPath,
                        Title = "AppId 或者 AppSecret 没有配置",
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

            var startTime = DateTime.Now;
            var translationResult = YoudaoTranslation.Translation(_settings, search);
            _context.API.LogInfo("YT", "Query Time: " + (DateTime.Now - startTime).TotalMilliseconds);

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


            if (translationResult.ResultList == null)
            {
                return null;
            }

            var results = new List<Result>();

            foreach (var itemModel in translationResult.ResultList)
            {
                results.Add(new Result
                {
                    Title = itemModel.Src,
                    SubTitle = itemModel.Dst,
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(itemModel.Dst);
                        return true;
                    }
                });
            }

            return results;
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
        }
    }
}