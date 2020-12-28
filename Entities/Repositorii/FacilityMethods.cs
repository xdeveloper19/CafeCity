using Entities.Context;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.AccountViewModels;
using Entities.ViewModels.FacilityViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repositorii
{
    public class FacilityMethods
    {
        private readonly ApplicationUserContext _userContext;
        private readonly CafeContext _applicationContext;
        public FacilityMethods(ApplicationUserContext userContext, CafeContext applicationContext)
        {
            _applicationContext = applicationContext;
            _userContext = userContext;
        }

        //public async Task<ServiceResponseObject<FacilityResponse>> Create(Guid userId, CreateFacilityViewModel model)
        //{
        //    var currentUser = await _applicationContext.Users.FindAsync(userId);

        //    ServiceResponseObject<FacilityResponse> DataContent = new ServiceResponseObject<FacilityResponse>();
        //    if (currentUser != null)
        //    {
        //        Facility team = new Facility
        //        {
        //            Name = model.Name,
        //            AdminId = currentUser.Id,
        //            Latitude = model.Latitude,
        //            Longitude = model.Longitude,
        //            Category = model.Category
        //        };

        //        _applicationContext.Facilities.Add(team);
        //        await _applicationContext.SaveChangesAsync();
        //        DataContent.ResponseData = new FacilityResponse
        //        {
        //            Id = team.Id,
        //            Name = team.Name,
        //            Category = team.Category,
        //            Latitude = team.Latitude,
        //            Longitude = team.Longitude
        //        };
        //        DataContent.Message = "Заведение успешно добавлено!";
        //        DataContent.Status = ResponseResult.OK;
        //        return DataContent;
        //    }
        //    DataContent.Message = "Упс, пользователь не найден.";
        //    DataContent.Status = ResponseResult.Error;
        //    return DataContent;
        //}

        public async Task<ServiceResponseObject<BaseResponseObject>> AddFavoriteFacility(AddPlaceViewModel model)
        {
            var currentUser = await _userContext.Users.FindAsync(model.UserId);
            var facility = _applicationContext.Facilities.Where(s => s.PlaceId == model.Id).FirstOrDefault();

            ServiceResponseObject<BaseResponseObject> DataContent = new ServiceResponseObject<BaseResponseObject>();
            if (currentUser == null)
            {
                DataContent.Message = "Пользователь не найден!";
                DataContent.Status = ResponseResult.Error;
                return DataContent;
            }

            if (facility == null)
            {
                facility = new Facility
                {
                    Name = model.Name,
                    PlaceId = model.Id,
                    Address = model.Address,
                    Description = model.Description,
                    Rate = model.Rating,
                    Phone = model.PhoneNumber
                };

                GeoData geo = new GeoData
                {
                    FacilityId = facility.Id,
                    Address = facility.Address,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude
                };

               // byte[] image = Convert.FromBase64String(model.PhotoData);
                MediaData media = new MediaData
                {
                    FacilityId = facility.Id,
                    //Content = image,
                    Path = model.PhotoId
                };
                var userFac = new UserFacility
                {
                    FacilityId = facility.Id,
                    UserId = currentUser.Id
                };

                _applicationContext.Media.Add(media);
                _applicationContext.Facilities.Add(facility);
                _applicationContext.UserHasFacility.Add(userFac);
                _applicationContext.GeoData.Add(geo);
                await _applicationContext.SaveChangesAsync();

                DataContent.Message = "Заведение успешно добавлено в список избранных!";
                DataContent.Status = ResponseResult.OK;
                return DataContent;
            }

            var user_place = _applicationContext.UserHasFacility.Where(p => p.FacilityId == facility.Id && p.UserId == model.UserId).FirstOrDefault();
            if (user_place == null)
            {
                user_place = new UserFacility()
                {
                    FacilityId = facility.Id,
                    UserId = model.UserId
                };

                _applicationContext.UserHasFacility.Add(user_place);
               
                _applicationContext.Update(facility);
                await _applicationContext.SaveChangesAsync();
                DataContent.Message = "Заведение успешно добавлено в список избранных!";
                DataContent.Status = ResponseResult.OK;
                return DataContent;
            }
            else
            {
                DataContent.Message = "Заведение уже добавлено в список избранных!";
                DataContent.Status = ResponseResult.Error;
                return DataContent;
            }
      
            
        }

        //public async Task<ServiceResponseObject<ListResponseObject<Facility>>> GetFacilitiesByCategory(Category category)
        //{
        //    ServiceResponseObject<ListResponseObject<Facility>> DataContent = new ServiceResponseObject<ListResponseObject<Facility>>();
        //    var facilities = await _applicationContext.Facilities.Where(f => f.Category == category)
        //        .Select(f => new Facility
        //        {
        //            AdminId = f.AdminId,
        //            Category = f.Category,
        //            Name = f.Name,
        //            Id = f.Id,
        //            Latitude = f.Latitude,
        //            Longitude = f.Longitude
        //        }).ToListAsync();

        //    if (facilities != null)
        //    {
        //        DataContent.ResponseData = new ListResponseObject<Facility>
        //        {
        //            Objects = facilities
        //        };
        //        DataContent.Message = "Успешно.";
        //        DataContent.Status = ResponseResult.OK;
        //        return DataContent;
        //    }
        //    DataContent.Message = "Таких заведений нет.";
        //    DataContent.Status = ResponseResult.Error;
        //    return DataContent;
        //}

        //public async Task<ServiceResponseObject<ListResponseObject<InLocationResponse>>> GetFacilitiesFromLocation(Guid userId, double Lon1, double Lat1)
        //{
        //    var currentUser = await _applicationContext.Users.FindAsync(userId);
        //    ServiceResponseObject<ListResponseObject<InLocationResponse>> DataContent = new ServiceResponseObject<ListResponseObject<InLocationResponse>>();
        //    GeometryMethods geometry = new GeometryMethods();

        //    if (currentUser != null)
        //    {
        //        var facilitiesInLocation = _applicationContext.Facilities //&& geometry.ArcInMeters(s.Latitude,s.Longitude,Lon1,Lat1)/1000.0 > 500.0)
        //            .Select(s => new InLocationResponse
        //            {
        //                FacilityId = s.Id,
        //                Name = s.Name,
        //                Distance = geometry.ArcInMeters(s.Latitude, s.Longitude, Lat1, Lon1) / 1000.0
        //            }).ToList();

        //        List<InLocationResponse> result = new List<InLocationResponse>();
        //        foreach (var Facility in facilitiesInLocation)
        //        {
        //            if (Facility.Distance < 500.0)
        //            {
        //                result.Add(Facility);
        //            }
        //        }

        //        DataContent.Message = "Успешно!";
        //        DataContent.Status = ResponseResult.OK;
        //        DataContent.ResponseData = new ListResponseObject<InLocationResponse>()
        //        {
        //            Objects = result
        //        };
        //        return DataContent;
        //    }

        //    DataContent.Message = "Пользователь не найден!";
        //    DataContent.Status = ResponseResult.Error;
        //    return DataContent;
        //}

        //public async Task<ServiceResponseObject<ListResponseObject<Facility>>> GetFacilities()
        //{
        //    var facilities = await _applicationContext.Facilities.Select(s => new Facility
        //    {
        //        AdminId = s.AdminId,
        //        Name = s.Name,
        //        Category = s.Category,
        //        Id = s.Id,
        //        Latitude = s.Latitude,
        //        Longitude = s.Longitude
        //    }).ToListAsync();
        //    ServiceResponseObject<ListResponseObject<Facility>> TeamData = new ServiceResponseObject<ListResponseObject<Facility>>();
        //    if (facilities != null)
        //    {
        //        TeamData.ResponseData = new ListResponseObject<Facility>
        //        {
        //            Objects = facilities
        //        };
        //        TeamData.Message = "Успешно!";
        //        TeamData.Status = ResponseResult.OK;
        //        return TeamData;
        //    }
        //    TeamData.Message = "Список всех заведений пуст.";
        //    TeamData.Status = ResponseResult.Error;
        //    return TeamData;
        //}

        public async Task<ServiceResponseObject<ListResponse<PlaceModel>>> GetUserFacilities(string id)
        {
            var currentUser = await _userContext.Users.FindAsync(id);
            ServiceResponseObject<ListResponse<PlaceModel>> TeamData = new ServiceResponseObject<ListResponse<PlaceModel>>();
            TeamData.ResponseData = new ListResponse<PlaceModel>();

            if (currentUser != null)
            {
                var userFacilities = _applicationContext.UserHasFacility.Where(f => f.UserId == id).ToList();
                if (userFacilities != null && userFacilities.Count != 0)
                {
                    foreach (var u_place in userFacilities)
                    {
                        var place = await _applicationContext.Facilities.FindAsync(u_place.FacilityId);
                        var media = _applicationContext.Media.Where(s => s.FacilityId == u_place.FacilityId).ToList();

                        TeamData.ResponseData.Objects.Add(new PlaceModel
                        { 
                            Address = place.Address,
                            Rating = place.Rate,
                            Path = media[0].Path,
                            Id = place.PlaceId,
                            Name = place.Name,
                            Descriptoin = place.Description
                        });
                    }
                    TeamData.Message = "Успешно!";
                    TeamData.Status = ResponseResult.OK;
                    return TeamData;
                }
                TeamData.Message = "У пользователя нет избранных заведений.";
                TeamData.Status = ResponseResult.Error;
                return TeamData;
            }
            TeamData.Message = "Пользователь не найден.";
            TeamData.Status = ResponseResult.Error;
            return TeamData;
        }

        //public async Task<ServiceResponseObject<BaseResponseObject>> Edit(Guid id, EditFacilityViewModel model)
        //{
        //    ServiceResponseObject<BaseResponseObject> DataContent = new ServiceResponseObject<BaseResponseObject>();
        //    var facility = await _applicationContext.Facilities.FindAsync(id);
        //    if (facility == null)
        //    {
        //        DataContent.Message = "Заведение не найдено.";
        //        DataContent.Status = ResponseResult.Error;
        //        return DataContent;
        //    }
        //    facility.Category = model.Type;
        //    facility.Latitude = model.Latitude;
        //    facility.Longitude = model.Longitude;
        //    facility.Name = model.Name;

        //    var Result = _applicationContext.Update(facility);
        //    await _applicationContext.SaveChangesAsync();

        //    DataContent.Message = "Успешно!";
        //    DataContent.Status = ResponseResult.OK;
        //    return DataContent;
        //}

        //public async Task<ServiceResponseObject<BaseResponseObject>> Delete(Guid id)
        //{
        //    var facility = await _applicationContext.Facilities.FindAsync(id);
        //    ServiceResponseObject<BaseResponseObject> DataContent = new ServiceResponseObject<BaseResponseObject>();
        //    if (facility != null)
        //    {
        //        _applicationContext.Facilities.Remove(facility);
        //        await _applicationContext.SaveChangesAsync();
        //        DataContent.Message = "Успешно!";
        //        DataContent.Status = ResponseResult.OK;
        //        return DataContent;
        //    }
        //    DataContent.Message = "Ошибка. Заведение не найдено.";
        //    DataContent.Status = ResponseResult.Error;
        //    return DataContent;
        //}
    }
}
