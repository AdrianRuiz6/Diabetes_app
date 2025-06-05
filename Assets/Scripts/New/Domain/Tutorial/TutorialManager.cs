using Master.Domain.Connection;
using Master.Domain.GameEvents;

public class TutorialManager
{
    public TutorialManager()
    {
        if(ConnectionManager.isFirstUsage == true){
            GameEvents_Tutorial.OnOpenTutorial?.Invoke();
            ConnectionManager.SetIsFirstUsage(false);
        }
        else
        {
            GameEvents_Tutorial.OnCloseTutorial?.Invoke();
        }
    }
}
