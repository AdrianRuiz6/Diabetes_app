using Master.Domain.PetCare;

namespace Master.Presentation.PetCare
{
    public class UI_Action_Insulin : UI_Actions_PetCare
    {
        private IPetCareManager _petCareManager;

        private void Start()
        {
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();
        }

        public override void UpdatedValueSlider(float value)
        {
            ValueTMP.text = value.ToString();
        }

        public override void SendInformation()
        {
            _petCareManager.ActivateInsulinAction(int.Parse(ValueTMP.text));

            base.SendInformation();
        }
    }
}