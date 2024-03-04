namespace MadisonCountySystem.Pages.DataClasses
{
    public class PlanStep
    {
        public int PlanStepID { get; set; }
        public String? PlanStepName { get; set; }
        public String? StepData { get; set; }
        public String? StepCreatedDate { get; set; }
        public String? DueDate { get; set; }
        public int OwnerID { get; set; }
        public int PlanID { get; set; }
    }
}
