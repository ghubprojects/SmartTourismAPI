using SmartTourismAPI.Domain.Models;

namespace SmartTourismAPI.Application.Abstractions.Providers;

public interface IGisOsmProvider {
    GridFile GetGridFile();
}
