using System.Threading.Tasks;
using Entities.Context;
using Entities.Repositorii;
using Entities.ViewModels.AccountViewModels;
using Entities.ViewModels.FacilityViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MvcUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private readonly CafeContext _cafeContext;
        private readonly ApplicationUserContext _userContext;

        public FacilityController(CafeContext cafeContext, ApplicationUserContext userContext)
        {
            _cafeContext = cafeContext;
            _userContext = userContext;
        }

        [HttpPost]
        [Route("CreatePlace")]
        public async Task<IActionResult> CreatePlace(AddPlaceViewModel model)
        {
            FacilityMethods AuthData = new FacilityMethods(_userContext, _cafeContext);
            var Result = await AuthData.AddFavoriteFacility(model);
            if (Result.Status == Entities.ViewModels.ResponseResult.OK)
                return Ok(Result);
            else
                return BadRequest(Result);
        }

        [HttpGet]
        [Route("FavoritePlaces")]
        public async Task<IActionResult> FavoritePlaces(string user_id)
        {
            FacilityMethods AuthData = new FacilityMethods(_userContext, _cafeContext);
            var Result = await AuthData.GetUserFacilities(user_id);
            if (Result.Status == Entities.ViewModels.ResponseResult.OK)
                return Ok(Result);
            else
                return BadRequest(Result);
        }

        //[HttpPost]
        //[Route("Create")]
        //public async Task<ServiceResponseObject<FacilityResponse>> Create(Guid userId, CreateFacilityViewModel model)
        //{
        //    FacilityMethods AuthData = new FacilityMethods(_applicationContext);
        //    var Result = await AuthData.Create(userId, model);
        //    return Result;
        //}
        ///**/
        //[HttpPost]
        //[Route("AddFavoriteFacility")]
        //public async Task<ServiceResponseObject<BaseResponseObject>> AddFavoriteFacility(Guid userId, Guid id)
        //{
        //    FacilityMethods ContentData = new FacilityMethods(_applicationContext);
        //    var Result = await ContentData.AddFavoriteFacility(userId, id);
        //    return Result;
        //}

        //[HttpGet]
        //[Route("GetFacilitiesByType")]
        //public async Task<ServiceResponseObject<ListResponseObject<Facility>>> GetFacilitiesByType(Category category)
        //{
        //    FacilityMethods ContentData = new FacilityMethods(_applicationContext);
        //    var Result = await ContentData.GetFacilitiesByCategory(category);
        //    return Result;
        //}

        //[HttpGet]
        //[Route("GetFacilitiesFromLocation")]
        //public async Task<ServiceResponseObject<ListResponseObject<InLocationResponse>>> GetFacilitiesFromLocation(Guid UserId, double Lon1, double Lat1)
        //{
        //    FacilityMethods ContentData = new FacilityMethods(_applicationContext);
        //    var Result = await ContentData.GetFacilitiesFromLocation(UserId, Lon1, Lat1);
        //    return Result;
        //}
        ///**/
        //[HttpGet]
        //[Route("GetUserFacilities")]
        //public async Task<ServiceResponseObject<ListResponseObject<Facility>>> GetUserFacilities(Guid userId)
        //{
        //    FacilityMethods ContentData = new FacilityMethods(_applicationContext);
        //    var Result = await ContentData.GetUserFacilities(userId);
        //    return Result;
        //}

        //[HttpGet]
        //[Route("GetFacilities")]
        //public async Task<ServiceResponseObject<ListResponseObject<Facility>>> GetFacilities()
        //{
        //    FacilityMethods ContentData = new FacilityMethods(_applicationContext);
        //    var Result = await ContentData.GetFacilities();
        //    return Result;
        //}

        //[HttpPut]
        //[Route("Edit")]
        //public async Task<ServiceResponseObject<BaseResponseObject>> Edit(Guid id, EditFacilityViewModel model)
        //{
        //    FacilityMethods TeamData = new FacilityMethods(_applicationContext);
        //    var Result = await TeamData.Edit(id, model);
        //    return Result;
        //}

        //[HttpDelete]
        //[Route("Delete")]
        //public async Task<ServiceResponseObject<BaseResponseObject>> Delete(Guid id)
        //{
        //    FacilityMethods TeamData = new FacilityMethods(_applicationContext);
        //    var Result = await TeamData.Delete(id);
        //    return Result;
        //}

    }
}