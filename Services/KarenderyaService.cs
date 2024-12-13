using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Interfaces;
using System.Security.Claims;
using TomNam.Exceptions;


namespace TomNam.Services
{
    public class KarenderyaService : IKarenderyaService
    {
        private readonly IKarenderyaRepository _karenderyaRepository;

        private readonly IFileUploadService _fileUploadService;
        private readonly IUserService _userService;

        public KarenderyaService(IKarenderyaRepository karenderyaRepository, IFileUploadService fileUploadService, IUserService userService)
        {
            _karenderyaRepository = karenderyaRepository;
            _fileUploadService = fileUploadService;
            _userService = userService;
        }

        public async Task<Karenderya> Create(ClaimsPrincipal User, KarenderyaRequestDTO.Create request)
        {
            var user = await _userService.GetUserFromToken(User);
            if (user == null)
            {
                throw new ApplicationExceptionBase(
                    "User is not authenticated.",
                    "Karenderya creation failed.",
                    StatusCodes.Status401Unauthorized
                );
            }
            var Karenderya = await GetByOwnerId(user.Id);

            if (Karenderya != null)
            {
                throw new ApplicationExceptionBase(
                    "User already has a karenderya.",
                    "Karenderya creation failed.",
                    StatusCodes.Status400BadRequest
                );
            }

            Karenderya = new Karenderya
            {
                UserId = user.Id,
                User = user,
                Name = request.Name,
                LocationStreet = request.LocationStreet,
                LocationBarangay = request.LocationBarangay,
                LocationCity = request.LocationCity,
                LocationProvince = request.LocationProvince,
                Description = request.Description,
                DateFounded = request.DateFounded
            };

            await _karenderyaRepository.Create(Karenderya);
            var OwnerProfile = await _userService.GetOwnerProfile(user.Id);
            await _userService.UpdateOwnerProfile(OwnerProfile!, Karenderya);
            return Karenderya;
        }

        public async Task<Karenderya?> GetById(Guid id)
        {
            return await _karenderyaRepository.GetByIdAsync(id);
        }

        public async Task<Karenderya?> GetByOwnerId(string id)
        {
            return await _karenderyaRepository.GetByOwnerId(id);
        }

        public async Task<Karenderya> Update(Guid KarenderyaId, KarenderyaRequestDTO.Update request, ClaimsPrincipal User)
        {
            var Karenderya = await GetById(KarenderyaId);

            if (Karenderya == null)
            {
                throw new ApplicationExceptionBase(
                    $"Karenderya with ID = {KarenderyaId} does not exist.",
                    "Karenderya update failed.",
                    StatusCodes.Status404NotFound
                );
            }

            var UserId = _userService.GetUserIdFromToken(User);

            if (Karenderya.UserId != UserId)
            {
                throw new ApplicationExceptionBase(
                    $"User is not authorized to update karenderya with ID = {KarenderyaId}. User is not the owner of the karenderya.",
                    "Karenderya update failed.",
                    StatusCodes.Status403Forbidden
                );
            }

            if (request.Name != null)
                Karenderya.Name = request.Name;

            if (request.LocationStreet != null)
                Karenderya.LocationStreet = request.LocationStreet;

            if (request.LocationBarangay != null)
                Karenderya.LocationBarangay = request.LocationBarangay;

            if (request.LocationCity != null)
                Karenderya.LocationCity = request.LocationCity;

            if (request.LocationProvince != null)
                Karenderya.LocationProvince = request.LocationProvince;

            if (request.Description != null)
                Karenderya.Description = request.Description;

            if (request.LogoPhoto != null)
            {
                string LogoPath = _fileUploadService.Upload(request.LogoPhoto, "Karenderya\\Logo");
                Karenderya.LogoPhoto = LogoPath;
            }

            if (request.CoverPhoto != null)
            {
                string CoverPhotoPath = _fileUploadService.Upload(request.CoverPhoto, "Karenderya\\Cover");
                Karenderya.CoverPhoto = CoverPhotoPath;
            }

            await _karenderyaRepository.Update(Karenderya);
            return Karenderya;
        }

        public async Task<List<Karenderya>> FilterKarenderyas(KarenderyaRequestDTO.Read filters)
        {
            var Karenderyas = await _karenderyaRepository.FilterKarenderya(filters);
            return Karenderyas;
        }

        public async Task<ProofOfBusiness> CreateProofOfBusiness(Guid KarenderyaId, KarenderyaRequestDTO.ProofOfBusinessCreateDTO request, ClaimsPrincipal User)
        {
            var Karenderya = await GetById(KarenderyaId);
            if (Karenderya == null)
            {
                throw new ApplicationExceptionBase(
                    $"Karenderya with id = {KarenderyaId} does not exist.",
                    "Karenderya proof of business creation failed.",
                    StatusCodes.Status404NotFound
                );
            }

            var UserId = _userService.GetUserIdFromToken(User);

            if (Karenderya.UserId != UserId)
            {
                throw new ApplicationExceptionBase(
                    $"User is not authorized to update karenderya with ID = {KarenderyaId}. User is not the owner of the karenderya.",
                    "Karenderya proof of business creation failed.",
                    StatusCodes.Status403Forbidden
                );
            }

            string ownerValidID1Path = _fileUploadService.Upload(request.OwnerValidID1, "Karenderya\\Proof\\ValidID");
            string ownerValidID2Path = _fileUploadService.Upload(request.OwnerValidID2, "Karenderya\\Proof\\ValidID");
            string businessPermitPath = _fileUploadService.Upload(request.BusinessPermit, "Karenderya\\Proof\\BusinessPermit");
            string BIRPermitPath = _fileUploadService.Upload(request.BIRPermit, "Karenderya\\Proof\\BIRPermit");

            var proof = new ProofOfBusiness
            {
                KarenderyaId = Karenderya.Id,
                Karenderya = Karenderya,
                OwnerValidID1 = ownerValidID1Path,
                OwnerValidID2 = ownerValidID2Path,
                BusinessPermit = businessPermitPath,
                BIRPermit = BIRPermitPath
            };

            await _karenderyaRepository.CreateProofOfBusiness(proof);
            return proof;
        }

        public async Task<ProofOfBusiness> GetProofOfBusiness(Guid KarenderyaId, ClaimsPrincipal User)
        {

            var Karenderya = await GetById(KarenderyaId);

            if (Karenderya == null)
            {
                throw new ApplicationExceptionBase(
                    "Karenderya not found.",
                    $"Karenderya with ID = {KarenderyaId} does not exist.",
                    StatusCodes.Status404NotFound
                );
            }

            var UserId = _userService.GetUserIdFromToken(User);

            if (Karenderya.UserId != UserId)
            {
                throw new ApplicationExceptionBase(

                    "Unauthorized",
                    $"User {User} is not authorized to view karenderya proof of business. User is not the owner of the karenderya.",
                    StatusCodes.Status403Forbidden
                );
            }

            var ProofOfBusiness = await _karenderyaRepository.GetProofOfBusiness(KarenderyaId);
            if (ProofOfBusiness == null)
            {
                throw new ApplicationExceptionBase(
                    "Karenderya Proof of Business is not found.",
                    $"No proof of business found for karenderya with ID = {KarenderyaId}.",
                    StatusCodes.Status404NotFound
                );
            }
            return ProofOfBusiness;
        }

        public async Task<ProofOfBusiness> UpdateProofOfBusiness(Guid KarenderyaId, KarenderyaRequestDTO.ProofOfBusinessUpdateDTO request, ClaimsPrincipal User)
        {
            var karenderya = await GetById(KarenderyaId);

            if (karenderya == null)
            {
                throw new ApplicationExceptionBase(
                    $"Karenderya with id = {KarenderyaId} does not exist.",
                    "Karenderya's proof of business update failed.",
                    StatusCodes.Status404NotFound
                );
            }

            var UserId = _userService.GetUserIdFromToken(User);

            if (karenderya.UserId != UserId)
            {
                throw new ApplicationExceptionBase(
                    $"User {User} is not authorized to update karenderya with ID = {KarenderyaId}. User is not the owner of the karenderya.",
                    "Karenderya proof of business update failed.",
                    StatusCodes.Status403Forbidden
                );
            }

            var proofOfBusiness = await GetProofOfBusiness(KarenderyaId, User);

            if (proofOfBusiness == null)
            {
                throw new ApplicationExceptionBase(
                    $"No proof of business found for karenderya with ID = {KarenderyaId}.",
                    "Proof is not found.",
                    StatusCodes.Status404NotFound
                );
            }

            if (request.OwnerValidID1 != null)
            {
                string ownerValidID1Path = _fileUploadService.Upload(request.OwnerValidID1, "Karenderya\\Proof\\ValidID");
                proofOfBusiness.OwnerValidID1 = ownerValidID1Path;
            }

            if (request.OwnerValidID2 != null)
            {
                string ownerValidID2Path = _fileUploadService.Upload(request.OwnerValidID2, "Karenderya\\Proof\\ValidID");
                proofOfBusiness.OwnerValidID2 = ownerValidID2Path;
            }

            if (request.BusinessPermit != null)
            {
                string businessPermitPath = _fileUploadService.Upload(request.BusinessPermit, "Karenderya\\Proof\\BusinessPermit");
                proofOfBusiness.BusinessPermit = businessPermitPath;
            }

            if (request.BIRPermit != null)
            {
                string BIRPermitPath = _fileUploadService.Upload(request.BIRPermit, "Karenderya\\Proof\\BIRPermit");
                proofOfBusiness.BIRPermit = BIRPermitPath;
            }

            await _karenderyaRepository.UpdateProofOfBusiness(proofOfBusiness);
            return proofOfBusiness;
        }

        public async Task<Karenderya> VerifyKarenderya(Guid KarenderyaId)
        {
            var Karenderya = GetById(KarenderyaId).Result;

            if (Karenderya == null)
            {
                throw new ApplicationExceptionBase(
                    $"Karenderya with ID = {KarenderyaId} does not exist.",
                    "Karenderya verification failed.",
                    StatusCodes.Status404NotFound
                );
            }

            Karenderya.IsVerified = true;
            await _karenderyaRepository.Update(Karenderya);
            return Karenderya;
        }
    }
}
