using AcademiaX_Data_Access.Models;
using System.Linq.Expressions;

namespace AcademiaX_Business.Dtos
{
	/// <summary>
	/// StudentService ve TeacherService'te birebir tekrarlanan PersonDTO projeksiyonu için tek kaynak.
	/// Expression olarak tanımlanır ki EF Core .Select(...) içinde SQL'e çevirebilsin
	/// (normal bir static metot burada işe yaramaz, IQueryable'ı IEnumerable'a düşürür).
	/// </summary>
	public static class PersonProjections
	{
		public static readonly Expression<Func<ApplicationUser, PersonDTO>> ToPersonDto = u => new PersonDTO
		{
			Id = u.Id,
			FullName = u.FirstName + " " + u.LastName,
			Email = u.Email,
			PhoneNumber = u.PhoneNumber,
			Image = u.Image
		};
	}
}
