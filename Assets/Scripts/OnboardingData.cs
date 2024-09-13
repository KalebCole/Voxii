using System.Collections.Generic;

public class OnboardingData
{
	public string PersonName { get; set; }
	public string LanguageProficiency { get; set; }
	public string LanguageToLearn { get; set; }
	public List<string> PhrasesToWorkOn { get; set; }
	public string Scene { get; set; }
	public Dictionary<string, string> SceneToRole { get; set; }

	public OnboardingData()
	{
		// Initialize with default values
		PersonName = "Kaleb";
		LanguageProficiency = "Beginner";
		LanguageToLearn = "English";
		PhrasesToWorkOn = new List<string> { "Greeting and Introduction", "Ordering Food and Drinks" };
		Scene = "Cafe";
		SceneToRole = new Dictionary<string, string>
		{
			{ "Cafe", "Barista" }
		};
	}
}