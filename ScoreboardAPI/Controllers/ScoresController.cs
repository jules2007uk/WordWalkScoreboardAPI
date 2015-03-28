using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ScoreboardAPI.Models;

namespace ScoreboardAPI.Controllers
{
    public class ScoresController : ApiController
    {
        private ScoreboardAPIContext db = new ScoreboardAPIContext();

        // GET api/Scores
        /// <summary>
        /// Gets the highest score from the leaderboard belonging to any user
        /// </summary>
        /// <returns></returns>
        public Score GetHighScore()
        {
            Score highScore = db.Scores.OrderByDescending(x => x.PlayerScore).FirstOrDefault();

            if (highScore == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return highScore;
            
        }

        // GET api/Scores/5
        /// <summary>
        /// Gets the highest score from the leaderboard for the player id supplied
        /// </summary>
        /// <param name="playerId">The ID of the player for which to retrieve the high score</param>
        /// <returns></returns>
        public Score GetHighScore(string playerId)
        {
            // go and fetch the highest score belonging to the player id supplied
            Score playerHighScore = db.Scores.OrderByDescending(x => x.PlayerScore).Where(x => x.PlayerID == playerId).FirstOrDefault();

            // throw 404 HTTP error if no score to return
            if (playerHighScore == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return playerHighScore;
        }  

        // POST api/Scores
        /// <summary>
        /// Submits a score to the database
        /// </summary>
        /// <param name="playerScore">Player's score</param>
        /// <param name="playerId">Player owner of the score</param>
        /// <returns></returns>
        public HttpResponseMessage PostScore(int playerScore, string playerId) 
        {            
            bool areParametersOk = true;

            // check if params passed in are ok

            // if params are ok then continue and add the score to the DB
            if (areParametersOk)
            {
                // create a new score object
                Score scoreToAdd = new Score();
                scoreToAdd.PlayerScore = playerScore;
                scoreToAdd.PlayerID = playerId;

                // call a data access layer method to save the score to the database
                db.Scores.Add(scoreToAdd);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = score.ID }));
                
                return response;                
            }
            else
            {
                // send back bad request status code
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Scores?id=5&accessCode=gW8quZqmzVJ6mAkxeDaCHdV1Zs7lr0iq
        /// <summary>
        /// Deletes specific score in the database
        /// </summary>
        /// <param name="id">The score ID to delete</param>
        /// <param name="accessCode">Access code required in order to carry out the deletion</param>
        /// <returns></returns>
        public HttpResponseMessage DeleteScore(int id, string accessCode)
        {
            // read in the secret access code held in the web config file
            string validAccessCode = System.Configuration.ConfigurationManager.AppSettings["AccessCodeDeleteScore"];

            // check if the access code supplied matches the code from the config
            if (accessCode == validAccessCode)
            {
                Score score = db.Scores.Find(id);
                if (score == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                db.Scores.Remove(score);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                return Request.CreateResponse(HttpStatusCode.OK, score);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The access code supplied does not have permission to delete scores.");
            }
            
        }

        // DELETE api/Scores?accessCode=pdOZMxFkFIunXt3Pbw6o63wK8QPELb6G
        /// <summary>
        /// Deletes all scores in the database
        /// </summary>
        /// <param name="accessCode">Access code required in order to carry out the deletion</param>
        /// <returns></returns>
        public HttpResponseMessage DeleteScore(string accessCode)
        {
            // read in the secret access code held in the web.config
            string validAccessCode = System.Configuration.ConfigurationManager.AppSettings["AccessCodeDeleteScore"];

            // check if the access code supplied matches the code from the config
            if (accessCode == validAccessCode)
            {

                // fetch all scores into a variable
                IEnumerable<Score> scores;
                scores = db.Scores;
                
                // call to remove all scores
                foreach (Score iteratedScore in scores)
                {
                    db.Scores.Remove(iteratedScore);
                }
                
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                return Request.CreateResponse(HttpStatusCode.OK, "All scores deleted.");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The access code supplied does not have permission to delete scores.");
            }

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}