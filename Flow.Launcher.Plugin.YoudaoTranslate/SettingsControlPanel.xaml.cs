using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.YoudaoTranslate;

public partial class SettingsControlPanel : UserControl
{
    private readonly PluginInitContext _context;
    private readonly Settings _settings;

    public SettingsControlPanel(PluginInitContext context, Settings settings)
    {
        _context = context;
        _settings = settings;
        InitializeComponent();

        InitView();

        TextBlockFlow.Text = """
                             1. 注册并登录: https://ai.youdao.com/
                             2. 在 https://ai.youdao.com/console/#/service-singleton/text-translation 创建应用
                             3. 选择 [自然语言翻译服务]
                             4. 选择 [文本翻译]
                             5. 接入方式: [API]
                             6. 创建成功后复制 应用ID(AppId) 和 应用密钥(AppSecret)
                             """;
    }

    private void InitView()
    {
        TextBoxUrl.Text = _settings.Url;
        TextBoxAppId.Text = _settings.AppId ?? "";
        TextBoxAppSecret.Text = _settings.AppSecret ?? "";
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        TextBlockTip.Text = "";
        var url = TextBoxUrl.Text;
        if (!url.StartsWith("https://") && !url.StartsWith("http://"))
        {
            TextBlockTip.Text = "URL 格式不正确";
            return;
        }

        var appId = TextBoxAppId.Text;
        if (string.IsNullOrEmpty(appId))
        {
            TextBlockTip.Text = "AppId 不能为空";
            return;
        }

        var appSecret = TextBoxAppSecret.Text;
        if (string.IsNullOrEmpty(appSecret))
        {
            TextBlockTip.Text = "AppSecret 不能为空";
            return;
        }

        _settings.Url = url;
        _settings.AppId = appId;
        _settings.AppSecret = appSecret;
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _settings.Url = "https://openapi.youdao.com/api";
        _settings.AppId = null;
        _settings.AppSecret = null;
        InitView();
    }
}