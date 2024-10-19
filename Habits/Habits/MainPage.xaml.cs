using Microsoft.Maui.Storage;
using System.Linq;

namespace Habits;

public partial class MainPage : ContentPage
{
    public List<Habit> Habits { get; set; }

    public MainPage()
    {
        InitializeComponent();
        Habits = new List<Habit>();
        LoadHabits();
        BindingContext = this;
    }


    private void LoadHabits()
    {
        var savedHabits = Preferences.Get("Habits", string.Empty);
        if (!string.IsNullOrEmpty(savedHabits))
        {
            Habits = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Habit>>(savedHabits);
        }

        HabitsListView.ItemsSource = null;
        HabitsListView.ItemsSource = Habits;
    }

    private void SaveHabits()
    {
        var habitList = Newtonsoft.Json.JsonConvert.SerializeObject(Habits);
        Preferences.Set("Habits", habitList);


        HabitsListView.ItemsSource = null;
        HabitsListView.ItemsSource = Habits;
    }


    private async void OnAddHabitClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Novo Hábito", "Nome do Hábito");
        var frequency = await DisplayPromptAsync("Frequência", "Quantas vezes por dia?");
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(frequency))
        {
            Habits.Add(new Habit { Name = name, Frequency = frequency, Progress = 0 });
            SaveHabits();
            await DisplayAlert("Sucesso", "Hábito adicionado!", "OK");
        }
    }

    private async void OnCompleteHabitClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var habitName = button?.CommandParameter as string;

        var selectedHabit = Habits.FirstOrDefault(h => h.Name == habitName);
        if (selectedHabit != null)
        {
            selectedHabit.Progress++;
            SaveHabits();
            await DisplayAlert("Sucesso", $"Progresso do hábito '{selectedHabit.Name}' foi atualizado para {selectedHabit.Progress}.", "OK");
        }
    }

    private async void OnDeleteHabitClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var habitName = button?.CommandParameter as string;

        var selectedHabit = Habits.FirstOrDefault(h => h.Name == habitName);
        if (selectedHabit != null)
        {
            bool confirm = await DisplayAlert("Apagar Hábito", $"Você quer apagar o hábito '{selectedHabit.Name}'?", "Sim", "Não");
            if (confirm)
            {
                Habits.Remove(selectedHabit);
                SaveHabits();
                await DisplayAlert("Sucesso", $"O hábito '{selectedHabit.Name}' foi apagado.", "OK");
            }
        }
    }
}
