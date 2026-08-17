namespace AcademiaX_Business.Dtos
{
	public class GradeDTO
	{
		public int Id { get; set; }
		public string StudentId { get; set; }
		public string StudentName { get; set; }
		public int CourseId { get; set; }
		public string ExamType { get; set; }
		public double Value { get; set; }
	}
}
