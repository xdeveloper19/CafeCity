
namespace Entities.ViewModels
{
    //Ответ аунтефикации
    public class AuthResponse: BaseResponseObject
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Distance { get; set; }
    }
}
