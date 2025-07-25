using Master.Domain.PetCare;
using Master.Infrastructure;

namespace Master.Presentation.PetCare
{
    public class UI_Action_Exercise : UI_Actions_PetCare
    {
        private IPetCareManager _petCareManager;

        private void Start()
        {
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();
            base.Start();
        }

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
            _petCareManager.ActivateExerciseAction(ValueTMP.text);

            base.SendInformation();
        }
    }
}