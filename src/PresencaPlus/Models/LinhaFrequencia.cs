namespace PresencaPlus;

public record LinhaFrequencia(Aluno Aluno, int Aulas, int Faltas)
{
    // RN01: (aulas com chamada − faltas) ÷ aulas com chamada × 100.
    public double? Percentual => Aulas == 0 ? null : (Aulas - Faltas) * 100.0 / Aulas;

    // RN02
    public string Situacao => Percentual switch
    {
        null => "Sem aulas",
        < 75 => "Abaixo de 75%",
        < 85 => "Atenção",
        _ => "Regular"
    };
}
