using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using H = System.Web.Http;
using EventServer;
using System.Data;
using EventServer.Entities;

namespace EventServer.Controllers
{
    public class EventsController : ApiController
    {

        DataTableProcesses dataTableProcesses;
        EventDataAccess dataaccess;

        public EventsController()
        {
            dataTableProcesses = new DataTableProcesses();
            dataaccess = new EventDataAccess();
            
        }

        [Authorize]
        public List<EventDetails> GetEventDetails()
        {
            DataTable eventDetails = new EventDataAccess().GetEventDetails();
            List<EventDetails> eventDetailsList = EventDetailEntities(eventDetails);

            return eventDetailsList;
        }

        [Authorize]
        public List<EventDetails> GetEventDetails(int eventId)
        {
            DataTable eventDetails = dataaccess.GetEventDetails(eventId);
            List<EventDetails> eventDetailsList = EventDetailEntities(eventDetails);

            return eventDetailsList;
        }

        [Authorize]
        public List<EventDetails> GetEventDetails(DateTime eventStartTime)
        {
            DataTable eventDetails = dataaccess.GetEventDetails(eventStartTime);
            List<EventDetails> eventDetailsList = EventDetailEntities(eventDetails);

            return eventDetailsList;
        }

        [Authorize]
        public List<SpecEventDetails> GetEventIdDetails(DateTime date)
        {
            DataTable eventDetails = dataaccess.GetEventDetails(date);
            List<SpecEventDetails> eventDetailsList = EventDetailForIdDate(eventDetails);
            return eventDetailsList;
        }

        [Authorize]
        public List<EventDetails> EventDetailEntities(DataTable eventDetails)
        {
            List<EventDetails> eventDetailsEntity = new List<EventDetails>();
            if (eventDetails != null && eventDetails.Rows.Count > 0)
            {
                foreach (DataRow row in eventDetails.Rows)
                {
                    int eventId;
                    DateTime startDate;

                    EventDetails eventDetail = new EventDetails
                    {
                        id = int.TryParse(dataTableProcesses.GetThisColumnValue("eventId", row), out eventId) ? Convert.ToInt16(dataTableProcesses.GetThisColumnValue("eventId", row)) : 0,
                        description = dataTableProcesses.GetThisColumnValue("eventDesc", row),
                        startTime = DateTime.TryParse(dataTableProcesses.GetThisColumnValue("eventStartTime", row), out startDate) ? Convert.ToDateTime(dataTableProcesses.GetThisColumnValue("eventStartTime", row)) : DateTime.MinValue,
                        type = dataTableProcesses.GetThisColumnValue("eventType", row),
                        title = dataTableProcesses.GetThisColumnValue("eventTitle", row),
                        venue = dataTableProcesses.GetThisColumnValue("eventVenue", row),
                        iconImageURL = dataTableProcesses.GetThisColumnValue("eventDspPic", row),
                        duration = dataTableProcesses.GetThisColumnValue("eventDuration", row)
                    };

                    eventDetailsEntity.Add(eventDetail);
                }
            }

            return eventDetailsEntity;
        }

        [Authorize]
        public List<SpecEventDetails> EventDetailForIdDate(DataTable eventDetails)
        {
            List<SpecEventDetails> eventDetailsEntity = new List<SpecEventDetails>();
            if (eventDetails != null && eventDetails.Rows.Count > 0)
            {
                foreach (DataRow row in eventDetails.Rows)
                {
                    int eventId;

                    SpecEventDetails eventDetail = new SpecEventDetails
                    {
                        id = int.TryParse(dataTableProcesses.GetThisColumnValue("eventId", row), out eventId) ? Convert.ToInt16(dataTableProcesses.GetThisColumnValue("eventId", row)) : 0,
                        lastUpdateOn = dataTableProcesses.GetThisColumnValue("lstupdatetime", row)
                    };

                    eventDetailsEntity.Add(eventDetail);
                }
            }

            return eventDetailsEntity;
        }



        }
}
