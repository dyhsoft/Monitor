using Admin.NET.Application.CoalMine.PersonInfo.Dtos;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.PersonInfo.Services;

public interface IPersonInfoService
{
    Task<SqlSugarPagedList<PersonInfoOutput>> GetPage(PersonInfoPageInput input);
    Task<PersonInfo> Get(long id);
    Task<long> Add(PersonInfoInput input);
    Task Update(PersonInfoInput input);
    Task Delete(long id);
}
