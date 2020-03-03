using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MvcUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private readonly CafeContext _applicationContext;
        public FacilityController(CafeContext applicationContext)
        {
            _applicationContext = applicationContext;
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