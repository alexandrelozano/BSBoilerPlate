using BSBoilerPlate.Data;

namespace BSBoilerPlate.Models.Events
{
    public class LoginEvent
    {
        public BSBoilerPlateContext _BSBoilerPlateContext;

        public Guid SessionGUID;

    }
}
