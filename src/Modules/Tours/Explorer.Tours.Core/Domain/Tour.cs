using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain;

public enum TourStatus
{
    Draft,
    Published,
    Archived
}
public class Tour : AggregateRoot
{
    public long CreatorId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Difficulty { get; private set; }
    public string[] Tags { get; private set; }
    public TourStatus Status { get; private set; }
    public double Price { get; private set; }
    public List<Keypoint> Keypoints { get; private set; }

    public Tour()
    {
        Title = "";
        Description = "";
        Tags = [];
        Status = TourStatus.Draft;
        Keypoints = [];
    }
    public Tour(long creatorId, string title, string description, int difficulty, string[] tags, TourStatus status = TourStatus.Draft, double price = 0)
    {
        if (difficulty < 1 || difficulty > 10) throw new ArgumentException("Difficulty must be between 1 and 10.");
        if (price < 0) throw new ArgumentException("Price cannot be negative.");
        Title = title;
        Description = description;
        Difficulty = difficulty;
        Tags = tags;
        Status = status;
        Price = price;
        CreatorId = creatorId;
        Keypoints = [];
    }

    public void Update(long creatorId, string title, string description, int difficulty, string[] tags, TourStatus status, double price)
    {
        CreatorId = creatorId;
        Title = title;
        Description = description;
        Difficulty = difficulty;
        Tags = tags;
        Status = status;
        Price = price;
    }

    public Keypoint AddKeypoint(Keypoint keypoint)
    {
        if (Status != TourStatus.Draft)
            throw new InvalidOperationException("Can only add keypoints to tour in draft");
        keypoint.SequenceNumber = GenerateKeypointSequenceNumber();
        Keypoints.Add(keypoint);
        return keypoint;
    }

    public Keypoint UpdateKeypoint(Keypoint updatedKeypoint)
    {
        if (Status != TourStatus.Draft)
            throw new InvalidOperationException("Can only update keypoints in tour in draft");

        var keypoint = Keypoints.FirstOrDefault(k => k.Id == updatedKeypoint.Id) ?? throw new NotFoundException("Keypoint not found");

        updatedKeypoint.SequenceNumber = keypoint.SequenceNumber;

        return keypoint.Update(updatedKeypoint);    
    }

    public void DeleteKeypoint(long keypointId)
    {
        if (Status != TourStatus.Draft)
            throw new InvalidOperationException("Can only delete keypoints from tour in draft");

        var keypoint = Keypoints.FirstOrDefault(k => k.Id == keypointId) ?? throw new NotFoundException($"Keypoint with id {keypointId} not found in tour");

        // Making it so only the last keypoint can be deleted so we don't have to care for other keypoint's SequenceNumbers :)
        if (keypointId != Keypoints.Last().Id)
            throw new InvalidOperationException("Can only delete last keypoint in tour");

        Keypoints.Remove(keypoint);
    }

    public void Publish()
    {
        ValidateForPublishing();

        Status = TourStatus.Published;
    }
    
    private void ValidateForPublishing()
    {
       // TODO
    }

    private int GenerateKeypointSequenceNumber()
    {
        return Keypoints.Count + 1;
    }
}
