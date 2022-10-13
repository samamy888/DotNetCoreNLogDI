# 在.net core 6 的 console app 上 DI 注入 NLog

[官方wiki](https://github.com/NLog/NLog.Extensions.Logging/wiki/NLog-configuration-with-appsettings.json)上也沒有教怎麼把NLog使用DI方式注進去

爬文發現有人實作了，就來記錄一下自己的版本。
開發版本是 .net 6

## 1. NuGet 安裝套件

- NLog
- NLog.Extensions.Logging

## 2. 創建NLog.config

創建在根目錄上，

相關config設定可以參考官方wiki [Configuration file](https://github.com/NLog/NLog/wiki/Configuration-file)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xsi:schemaLocation="NLog NLog.xsd"
      autoReload="true"
      throwExceptions="false"
      internalLogLevel="Off" internalLogFile="NLog\all_log.log">

  <targets>
    <!--檔案日誌，archive相關引數：檔案拆分，每100M拆分一個新檔案-->
    <target xsi:type="File"
          name="all_log"
          fileName="NLog\${shortdate}\${uppercase:${level}}.log"
          layout="${longdate}|${logger}|${uppercase:${level}}|${message} ${exception}"
          archiveFileName="NLog\${shortdate}\${uppercase:${level}}${shortdate}.{####}.log"
          archiveNumbering="Rolling"
          archiveAboveSize="10485760"
          concurrentwrites="true"
          maxArchiveFiles="100"
              />

  </targets>


  <rules>
    <!-- add your logging rules here -->
    <!--路由順序會對日誌列印產生影響。路由匹配邏輯為順序匹配。-->
    <logger name="*" minlevel="Information" writeTo="all_log" />
  </rules>
</nlog>
```

## 3. Program.cs 部分

```csharp
using DotNetCoreNLogDI;
using NLog.Extensions.Logging;

static void ConfigureServices(IServiceCollection services)
{
    // build config
    IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

    // configure logging
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddConsole();
        builder.AddDebug();
        builder.AddNLog(config.GetSection("NLog"));
    });

    // add app
    services.AddTransient<App>();
}

var services = new ServiceCollection();
ConfigureServices(services);

using var serviceProvider = services.BuildServiceProvider();

// entry to run app
await serviceProvider.GetService<App>()!.Run();
```

## 4. App.cs 部分
```csharp
public class App
{
    private readonly ILogger<App> _logger;
    public App(ILogger<App> logger)
    {
        _logger = logger;
    }
    public async Task Run()
    {
        _logger.LogInformation("LogInformation");
        _logger.LogWarning("LogWarning");
    }
}
```

## 5. 結果範例

會分別記錄為兩個log檔:
- INFO.log
- WARN.log
  
```
2022-10-13 11:55:14.7064|DotNetCoreNLogDI.App|INFO|LogInformation 
2022-10-13 11:55:14.7744|DotNetCoreNLogDI.App|WARN|LogWarning 
```
## Github Repo

[Github Repo 位置](https://github.com/samamy888/DotNetCoreNLogDI)

## 參考資料

[https://github.com/iSatishYadav/net-core-console-nlog-with-di/blob/master/README.md](https://github.com/iSatishYadav/net-core-console-nlog-with-di/blob/master/README.md)


