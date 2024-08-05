using Domain.Entities;
using Domain.Interface.Service;
using Infrastructure.Services;

namespace Infrastructure.API
{
    public class UserCrudServices
    {
        private readonly IDataService<User> _crudServices;

        public UserCrudServices()
        {
            _crudServices = new GenericDataService<User>(new ApplicationContextFactory());
        }

        public async Task<User> AddBrand(string email, string password)
        {
            try
            {
                if (email == string.Empty)
                {
                    throw new Exception("Student Name Cannot be Empty");
                }
                else
                {
                    User br = new User
                    {
                        Email = email,
                        Password = password
                    };
                    return await _crudServices.Create(br);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteBrand(int id)
        {
            try
            {
                User delete = await SearchBrandbyID(id);

                return await _crudServices.Delete(delete);



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ICollection<User>> ListBrands()
        {
            try
            {
                return (ICollection<User>)await _crudServices.GetAll();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public Task<User> SearchBrandbyID(int ID)
        {
            try
            {
                return _crudServices.Get(ID);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<User>> SearchBrandByName(string stname)
        {
            try
            {
                var listbrand = await ListBrands();
                return listbrand.Where(x => x.Email.StartsWith(stname)).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task<User> UpdateBrand(int id, string email, string course)
        {
            try
            {
                User br = await SearchBrandbyID(id);
                br.Email= email;
                return await _crudServices.Update(br);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
    }
}