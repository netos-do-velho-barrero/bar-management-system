namespace ControleDeBar.Aplicacao.Modulos.ModuloMesa;

public record ListarMesaDto(
    Guid Id,
    int Numero,
    int QuantidadeLugares,
    string Status
);

public record CadastrarMesaDto(
    int Numero,
    int QuantidadeLugares
);

public record EditarMesaDto(
    Guid Id,
    int Numero,
    int QuantidadeLugares
);

public record DetalhesMesaDto(
    Guid Id,
    int Numero,
    int QuantidadeLugares,
    string Status
);
