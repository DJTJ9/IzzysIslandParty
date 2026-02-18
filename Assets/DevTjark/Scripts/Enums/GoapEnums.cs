public enum Beliefs
{
    Nothing,
    FindNextPosition,
    ReachCheckPoint1,
    ReachCheckPoint2,
    ReachCheckPoint3,
    ReachCheckPoint4,
    ReachCheckPoint5,
    ReachCheckPoint6,
    ReachCheckPoint7,
    ReachCheckPoint8,
    ReachCheckPoint9,
    ReachCheckPoint10,
    WinGame,
    AttackingPlayer1,
    AttackingPlayer2,
    AttackingPlayer3,
    
    // Location Beliefs
    FinishInReach,
    CheckPoint1InReach,
    CheckPoint2InReach,
    CheckPoint3InReach,
    CheckPoint4InReach,
    CheckPoint5InReach,
    CheckPoint6InReach,
    CheckPoint7InReach,
    CheckPoint8InReach,
    CheckPoint9InReach,
    CheckPoint10InReach,
    Player1Close,
    Player2Close,
    Player3Close
}

public enum Actions
{
    Relax,
    MoveToNextPosition,
    GoForCheckPoint1,
    GoForCheckPoint2,
    GoForCheckPoint3,
    GoForCheckPoint4,
    GoForCheckPoint5,
    GoForCheckPoint6,
    GoForCheckPoint7,
    GoForCheckPoint8,
    GoForCheckPoint9,
    GoForCheckPoint10,
    GoForFinish,
    AttackPlayer1,
    AttackPlayer2,
    AttackPlayer3
}

public enum Goals
{
    ChillOut,
    ComingCloserToFinish,
    GoForCheckPoint1,
    GoForCheckPoint2,
    GoForCheckPoint3,
    GoForCheckPoint4,
    GoForCheckPoint5,
    GoForCheckPoint6,
    GoForCheckPoint7,
    GoForCheckPoint8,
    GoForCheckPoint9,
    GoForCheckPoint10,
    GoForFinish,
    AttackPlayer1,
    AttackPlayer2,
    AttackPlayer3
}
