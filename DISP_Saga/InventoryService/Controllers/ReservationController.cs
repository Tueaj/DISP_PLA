using System.Collections.Generic;
using InventoryService.Models;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        [HttpGet]
        public IEnumerable<Reservation> GetReservations()
        {
            return _reservationRepository.GetReservations();
        }

        [HttpGet("{id}")]
        public ActionResult<Reservation> GetReservation(string id)
        {
            Reservation? reservation = _reservationRepository.GetReservation(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }
    }
}