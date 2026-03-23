// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何法律责任！

using System.Text.RegularExpressions;

namespace Admin.NET.Core.Service;

/// <summary>
/// 煤矿自动导入服务
/// 从数据文件夹扫描并自动创建煤矿
/// </summary>
public class CoalMineAutoImportService : ITransient
{
    private readonly SqlSugarRepository<CoalMine> _coalMineRep;
    private readonly ILogger<CoalMineAutoImportService> _logger;

    public CoalMineAutoImportService(
        SqlSugarRepository<CoalMine> coalMineRep,
        ILogger<CoalMineAutoImportService> logger)
    {
        _coalMineRep = coalMineRep;
        _logger = logger;
    }

    /// <summary>
    /// 从文件夹扫描并自动导入煤矿
    /// </summary>
    /// <param name="folderPaths">要扫描的文件夹路径列表</param>
    public async Task<int> AutoImportFromFolders(List<string> folderPaths)
    {
        var importedCount = 0;

        foreach (var folderPath in folderPaths)
        {
            if (!Directory.Exists(folderPath))
            {
                _logger.LogWarning("文件夹不存在: {FolderPath}", folderPath);
                continue;
            }

            var files = Directory.GetFiles(folderPath, "*.txt", SearchOption.TopDirectoryOnly);
            _logger.LogInformation("扫描文件夹: {FolderPath}, 文件数: {Count}", folderPath, files.Length);

            // 提取所有煤矿信息
            var mineDict = new Dictionary<string, string>(); // Code -> Name

            foreach (var file in files)
            {
                try
                {
                    var firstLine = File.ReadLines(file).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(firstLine)) continue;

                    // 解析文件头: 煤矿编号;煤矿名称;时间
                    var parts = firstLine.Split(';');
                    if (parts.Length >= 2)
                    {
                        var code = parts[0].Trim();
                        var name = parts[1].Trim();

                        if (!string.IsNullOrEmpty(code) && !mineDict.ContainsKey(code))
                        {
                            mineDict[code] = string.IsNullOrEmpty(name) ? code : name;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "解析文件失败: {File}", file);
                }
            }

            // 批量创建煤矿
            foreach (var (code, name) in mineDict)
            {
                var existing = await _coalMineRep.AsQueryable()
                    .Where(m => m.Code == code)
                    .FirstAsync();

                if (existing == null)
                {
                    var coalMine = new CoalMine
                    {
                        Code = code,
                        Name = name,
                        Address = "",
                        FtpPath = "",
                        DataType = "",
                        Enabled = 1,
                        Remark = $"自动从数据文件导入: {folderPath}"
                    };

                    await _coalMineRep.InsertAsync(coalMine);
                    importedCount++;
                    _logger.LogInformation("自动创建煤矿: {Code} - {Name}", code, name);
                }
            }
        }

        _logger.LogInformation("煤矿自动导入完成，共导入 {Count} 个煤矿", importedCount);
        return importedCount;
    }

    /// <summary>
    /// 从指定文件夹自动导入煤矿（默认工作文件夹）
    /// </summary>
    public async Task<int> AutoImportFromWorkFolders()
    {
        var workFolders = new List<string>
        {
            "/home/yourname/.openclaw/work/安全监测",
            "/home/yourname/.openclaw/work/人员定位",
            "/home/yourname/.openclaw/work/水害"
        };

        return await AutoImportFromFolders(workFolders);
    }
}
