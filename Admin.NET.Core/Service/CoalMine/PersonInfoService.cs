// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 人员信息服务
/// </summary>
[ApiDescriptionSettings("PersonInfo", Description = "人员信息")]
public class PersonInfoService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<PersonInfo> _personInfoRep;

    public PersonInfoService(SqlSugarRepository<PersonInfo> personInfoRep)
    {
        _personInfoRep = personInfoRep;
    }

    /// <summary>
    /// 分页查询人员信息
    /// </summary>
    [DisplayName("分页查询人员信息")]
    public async Task<SqlSugarPagedList<PersonInfoOutput>> GetPage([FromQuery] PagePersonInfoInput input)
    {
        return await _personInfoRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.PersonName), u => u.PersonName.Contains(input.PersonName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.CardId), u => u.CardId.Contains(input.CardId))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Department), u => u.Department.Contains(input.Department))
            .WhereIF(!string.IsNullOrWhiteSpace(input.WorkType), u => u.WorkType.Contains(input.WorkType))
            .WhereIF(input.Status.HasValue, u => u.Status == input.Status)
            .OrderBy(u => u.Id)
            .Select<PersonInfoOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取人员详情
    /// </summary>
    [DisplayName("获取人员详情")]
    public async Task<PersonInfoOutput> Get(long id)
    {
        return await _personInfoRep.AsQueryable()
            .Where(u => u.Id == id)
            .Select<PersonInfoOutput>()
            .FirstAsync();
    }

    /// <summary>
    /// 新增人员
    /// </summary>
    [DisplayName("新增人员")]
    public async Task<long> Add(AddPersonInfoInput input)
    {
        var entity = input.Adapt<PersonInfo>();
        return await _personInfoRep.InsertReturnIdentityAsync(entity);
    }

    /// <summary>
    /// 更新人员
    /// </summary>
    [DisplayName("更新人员")]
    public async Task Update(UpdatePersonInfoInput input)
    {
        var entity = input.Adapt<PersonInfo>();
        await _personInfoRep.AsUpdateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除人员
    /// </summary>
    [DisplayName("删除人员")]
    public async Task Delete(long id)
    {
        await _personInfoRep.DeleteByIdAsync(id);
    }

    /// <summary>
    /// 获取部门列表
    /// </summary>
    [DisplayName("获取部门列表")]
    public async Task<List<string>> GetDepartments([FromQuery] long mineId)
    {
        return await _personInfoRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .Select(u => u.Department)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>
    /// 获取工种列表
    /// </summary>
    [DisplayName("获取工种列表")]
    public async Task<List<string>> GetWorkTypes([FromQuery] long mineId)
    {
        return await _personInfoRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .Select(u => u.WorkType)
            .Distinct()
            .ToListAsync();
    }
}

/// <summary>
/// 人员分页输入
/// </summary>
public class PagePersonInfoInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 工种
    /// </summary>
    public string WorkType { get; set; }

    /// <summary>
    /// 状态:1-在岗,2-请假,3-离职
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 新增人员输入
/// </summary>
public class AddPersonInfoInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 工种
    /// </summary>
    public string WorkType { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    public string IdCard { get; set; }

    /// <summary>
    /// 入职日期
    /// </summary>
    public DateTime? JoinDate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

/// <summary>
/// 更新人员输入
/// </summary>
public class UpdatePersonInfoInput : AddPersonInfoInput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// 人员信息输出
/// </summary>
public class PersonInfoOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 工种
    /// </summary>
    public string WorkType { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    public string IdCard { get; set; }

    /// <summary>
    /// 岗位状态:1-在岗,2-请假,3-离职
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName => Status switch
    {
        1 => "在岗",
        2 => "请假",
        3 => "离职",
        _ => "未知"
    };

    /// <summary>
    /// 入职日期
    /// </summary>
    public DateTime? JoinDate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}
