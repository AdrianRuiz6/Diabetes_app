using Master.Domain.PetCare;

namespace Master.Presentation.PetCare
{
    public class UI_Action_Exercise : UI_Actions_PetCare
    {
        public override void UpdatedValueSlider(float value)
        {
            switch (value)
            {
                case 1:
                    ValueTMP.text = "Intensidad baja";
                    break;
                case 2:
                    ValueTMP.text = "Intensidad media";
                    break;
                case 3:
                    ValueTMP.text = "Intensidad alta";
                    break;
            }
        }

        public override void SendInformation()
        {
            AttributeManager.Instance.ActivateExerciseAction(ValueTMP.text);

            base.SendInformation();
        }
    }
}