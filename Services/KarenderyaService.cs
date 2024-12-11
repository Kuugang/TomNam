using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Interfaces;


namespace TomNam.Services
{
	public class KarenderyaService : IKarenderyaService
	{
		private readonly IKarenderyaRepository _karenderyaRepository;

		private readonly IFileUploadService _fileUploadService;

		public KarenderyaService(IKarenderyaRepository karenderyaRepository, IFileUploadService fileUploadService)
		{
			_karenderyaRepository = karenderyaRepository;
			_fileUploadService = fileUploadService;
		}

		public async Task<Karenderya> Create(KarenderyaRequestDTO.Create request, User user)
		{
			var karenderya = new Karenderya
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

			return await _karenderyaRepository.Create(karenderya);
		}

		public async Task<Karenderya?> GetById(Guid id)
		{
			return await _karenderyaRepository.GetByIdAsync(id);
		}

		public async Task<Karenderya?> GetByOwnerId(string id)
		{
			return await _karenderyaRepository.GetByOwnerId(id);
		}

		public async Task<Karenderya> Update(Karenderya karenderya, KarenderyaRequestDTO.Update request)
		{
			if (request.Name != null)
				karenderya.Name = request.Name;

			if (request.LocationStreet != null)
				karenderya.LocationStreet = request.LocationStreet;

			if (request.LocationBarangay != null)
				karenderya.LocationBarangay = request.LocationBarangay;

			if (request.LocationCity != null)
				karenderya.LocationCity = request.LocationCity;

			if (request.LocationProvince != null)
				karenderya.LocationProvince = request.LocationProvince;

			if (request.Description != null)
				karenderya.Description = request.Description;

			if (request.LogoPhoto != null)
			{
				string LogoPath = _fileUploadService.Upload(request.LogoPhoto, "Karenderya\\Logo");
				karenderya.LogoPhoto = LogoPath;
			}

			if (request.CoverPhoto != null)
			{
				string CoverPhotoPath = _fileUploadService.Upload(request.CoverPhoto, "Karenderya\\Cover");
				karenderya.CoverPhoto = CoverPhotoPath;
			}

			await _karenderyaRepository.Update(karenderya);
			return karenderya;
		}

		public async Task<List<KarenderyaResponseDTO>> FilterKarenderya(KarenderyaRequestDTO.Read filters)
		{
			var karenderyas = await _karenderyaRepository.FilterKarenderya(filters);

			var response = karenderyas.Select(k => new KarenderyaResponseDTO
			{
				Id = k.Id,
				UserId = k.UserId,
				Name = k.Name,
				LocationStreet = k.LocationStreet,
				LocationBarangay = k.LocationBarangay,
				LocationCity = k.LocationCity,
				LocationProvince = k.LocationProvince,
				Description = k.Description,
				LogoPhoto = k.LogoPhoto,
				CoverPhoto = k.CoverPhoto,
				DateFounded = k.DateFounded,
				IsVerified = k.IsVerified,
				Rating = k.Rating,
			}).ToList();

			return response;
		}

		public async Task<ProofOfBusiness> CreateProofOfBusiness(Karenderya karenderya, KarenderyaRequestDTO.ProofOfBusinessCreateDTO request)
		{
			string ownerValidID1Path = _fileUploadService.Upload(request.OwnerValidID1, "Karenderya\\Proof\\ValidID");
			string ownerValidID2Path = _fileUploadService.Upload(request.OwnerValidID2, "Karenderya\\Proof\\ValidID");
			string businessPermitPath = _fileUploadService.Upload(request.BusinessPermit, "Karenderya\\Proof\\BusinessPermit");
			string BIRPermitPath = _fileUploadService.Upload(request.BIRPermit, "Karenderya\\Proof\\BIRPermit");

			var proof = new ProofOfBusiness
			{
				KarenderyaId = karenderya.Id,
				Karenderya = karenderya,
				OwnerValidID1 = ownerValidID1Path,
				OwnerValidID2 = ownerValidID2Path,
				BusinessPermit = businessPermitPath,
				BIRPermit = BIRPermitPath
			};
			await _karenderyaRepository.CreateProofOfBusiness(proof);
			return proof;
		}

		public async Task<ProofOfBusiness?> GetProofOfBusiness(Guid karenderyaId)
		{
			return await _karenderyaRepository.GetProofOfBusiness(karenderyaId);
		}

		public async Task<ProofOfBusiness> UpdateProofOfBusiness(ProofOfBusiness proofOfBusiness, KarenderyaRequestDTO.ProofOfBusinessUpdateDTO request)
		{
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
	}
}
