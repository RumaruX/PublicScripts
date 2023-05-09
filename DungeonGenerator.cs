using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room{
    private int _roomType = 0;
    private List<int> _doorsLocations = new List<int>();

    public int RoomType{
        get{
            return _roomType;
        }
        set{
            _roomType = value;
        }
    }

    public List<int> DoorsLocations{
        get{
            return _doorsLocations;
        }
        set{
            _doorsLocations = value;
        }
    }

    public string GetRoomInformations(){
        string ch = $"\tRoom type : {_roomType}\n\tDoors :";
        foreach(int doorLocation in _doorsLocations){
            ch += $" {doorLocation},";
        }
        if(_doorsLocations.Count == 0){
            ch += " None";
        }else{
            ch = ch.Substring(0, ch.Length - 1);
        }
        return ch + ".";
    }
}

public class DungeonGenerator : MonoBehaviour
{

    #region Variables

    /*

    ====================================================================================
    ROOMS INFORMATIONS
    ====================================================================================
    Pour les types de 
    0 = Salle en dohors du donjon, inutile
    1 = Salle en dohors du donjon, sert aux murs
    2 = salle a prendre pour aller au boss
    2 = salle supplementaire
    4 = salle d'enigme
    5 = salle de miniboss
    6 = salle de tresor
    9 = salle de boss
    Pour les portes
    1 = gauche
    2 = haut
    3 = bas
    4 = droite
    ====================================================================================

    */

    [Range(3, 7)]
    [SerializeField] private int dungeonWidth = 5;
    [Range(3, 9)]
    [SerializeField] private int dungeonLength = 7;
    [SerializeField] private int minPathSize = 7;

    [SerializeField] private bool reloadDungeon = false; // Ne sert que pour tester un nouveau donjon

    [SerializeField] private GameObject floor;
    [SerializeField] private Transform floorParent;
    [SerializeField] private GameObject door;
    [SerializeField] private Transform doorParent;
    [SerializeField] private GameObject wall;
    [SerializeField] private Transform wallParent;

    private List<List<Room>> _dungeonGrid;
    private bool _startRight = true;
    private (int, int) _bossRoom;
    private (int, int) _startRoom;
    private List<(int, int)> _actualPath;

    #endregion



    #region Properties

    #endregion



    #region Built-in Methods

    void Start()
    {
        CreateDungeon();
    }

    void OnValidate(){
        if(Application.isPlaying){ // Si en train de jouer et changement dans les variables
            CreateDungeon();
        }
    }

    #endregion



    #region Custom Methods

    List<(int, int)> ShuffleList(List<(int, int)> listToShuffle){ // Malangeur de liste
        List<(int, int)> shuffledList = new List<(int, int)>(); // Liste terminee
        List<int> remainingPlaces = new List<int>(); // Liste de numeros(de 0 au nombre d'items dans la liste a melanger)
        for(int i = 0; i < listToShuffle.Count; i++){
            remainingPlaces.Add(i); // Remplis avec les numeros
            shuffledList.Add((-1, -1)); // Remplis avec une case inexistante
        }

        foreach((int, int) item in listToShuffle){ // Prends un numeo au hasard, le retire de la liste des numeros, et change l'item a l'index du numero pour l'item actuel de la liste a melanger
            int randomNum = remainingPlaces[Random.Range(0, remainingPlaces.Count)];
            remainingPlaces.Remove(randomNum);
            shuffledList[randomNum] = item;
        }

        return shuffledList; // Envoie la liste melangee
    }

    void CreateDungeon(){ // Creation du donjon

        // Determine si on commence a gauche ou a droite (si dungeonLength est pair, on utilise la variable)

        if(Random.Range(0, 2) == 0) _startRight = !_startRight;

        _actualPath = new List<(int, int)>();
        if(minPathSize <= (dungeonLength * dungeonWidth)/2){ // Check si la taille minimum du chemin est possible
            while(_actualPath.Count < minPathSize){ // Tant que la taille du chemina actuel est plus petit que la taille minimum voulue, recree le donjon

                // Crees la grille du donjon

                _dungeonGrid = new List<List<Room>>();

                for(int z = 0; z < dungeonWidth; z++){
                    _dungeonGrid.Add(new List<Room>());
                    for(int x = 0; x < dungeonLength; x++){
                        _dungeonGrid[z].Add(new Room());
                        _dungeonGrid[z][x].RoomType = 0;
                    }
                }

                // Place la salle de depart

                if(dungeonLength % 2 == 0){
                    if(_startRight){
                        _startRoom = (dungeonWidth-1, dungeonLength/2);
                    }else{
                        _startRoom = (dungeonWidth-1, dungeonLength/2-1);
                    }
                }else{
                    _startRoom = (dungeonWidth-1, dungeonLength/2);
                }

                // Place la salle du boss

                ChooseBossRoom();

                // Cree le chemin

                _actualPath = new List<(int, int)>();
                ChoosePath(_startRoom.Item1, _startRoom.Item2);
                if(_actualPath.Count >= minPathSize){
                    break;
                }
            }
        }else{ // Sinon, ne continue pas a generer
            Debug.LogError("MinPathSize is too big for this dungeon.");
            return;
        }
        
        // Trasnformationd es salles 1 en 0 (on en aura besoin pour apres)

        for(int z = 0; z < dungeonWidth; z++){
            for(int x = 0; x < dungeonLength; x++){
                if(_dungeonGrid[z][x].RoomType == 1){
                    _dungeonGrid[z][x].RoomType = 0;
                }
            }
        }

        // Ajout des portes aux salles

        AddDoorsInformations();

        // Ajout de salles supplementaires, non necessaires

        for(int z = 0; z < dungeonWidth; z++){
            for(int x = 0; x < dungeonLength; x++){
                if(_dungeonGrid[z][x].RoomType == 0){
                    List<(int, int)> neighboursList = GetNeighbours(z, x);
                    bool possibleNewRoom = false;
                    bool noBoss = true;
                    (int, int) choosedNeighbour = (0, 0);
                    foreach((int, int) neighbour in neighboursList){
                        if(_dungeonGrid[neighbour.Item1][neighbour.Item2].RoomType != 0){
                            if(!possibleNewRoom){
                                choosedNeighbour = neighbour;
                            }
                            possibleNewRoom = true;
                            if(_dungeonGrid[neighbour.Item1][neighbour.Item2].RoomType == 9){
                                noBoss = false;
                            }
                        }
                    }
                    if(possibleNewRoom && noBoss){
                        if(Random.Range(0, 100) < 70){
                            _dungeonGrid[z][x].RoomType = 3;
                            List<int> ancientNeighbourDoorsList = _dungeonGrid[choosedNeighbour.Item1][choosedNeighbour.Item2].DoorsLocations;
                            if(choosedNeighbour.Item2 > x){ // Droite
                                _dungeonGrid[z][x].DoorsLocations = new List<int>(){4};
                                ancientNeighbourDoorsList.Add(1);
                            }
                            if(choosedNeighbour.Item2 < x){ // Gauche
                                _dungeonGrid[z][x].DoorsLocations = new List<int>(){1};
                                ancientNeighbourDoorsList.Add(4);
                            }
                            if(choosedNeighbour.Item1 > z){ // Bas
                                _dungeonGrid[z][x].DoorsLocations = new List<int>(){3};
                                ancientNeighbourDoorsList.Add(2);
                            }
                            if(choosedNeighbour.Item1 < z){ // Haut
                                _dungeonGrid[z][x].DoorsLocations = new List<int>(){2};
                                ancientNeighbourDoorsList.Add(3);
                            }
                        }
                    }
                }
            }
        }

        // Transformation des salles 0 a cote des autres salles en salles 1 (pour mettre les murs etc) et transformations de certaines salles en salles d'enigme, de miniboss et de tresor

        for(int z = 0; z < dungeonWidth; z++){
            for(int x = 0; x < dungeonLength; x++){
                if(_dungeonGrid[z][x].RoomType == 0){
                    List<(int, int)> neighboursList = GetNeighbours(z, x);
                    foreach((int, int) neighbour in neighboursList){
                        if(_dungeonGrid[neighbour.Item1][neighbour.Item2].RoomType != 0 && _dungeonGrid[neighbour.Item1][neighbour.Item2].RoomType != 1){
                            _dungeonGrid[z][x].RoomType = 1;
                        }
                    }
                }
                if(_dungeonGrid[z][x].RoomType == 3 && _dungeonGrid[z][x].DoorsLocations.Count == 1){
                    List<int> possibleRooms = new List<int>(){4, 5, 6};
                    if(Random.Range(0, 100) < 65){
                        _dungeonGrid[z][x].RoomType = possibleRooms[Random.Range(0, possibleRooms.Count)];
                    }
                }
            }
        }

        // Cree le donjon en 3D

        CreateInGame();

        // Montre la grille (Debug)
        
        // ShowGrid();
    }

    void ChooseBossRoom(){ // Choix automatique de l'emplacement de la salle du boss
        if(Random.Range(0, 2) == 0){
            if(Random.Range(0, 2) == 0){ // Cote gauche
                _bossRoom = (Random.Range(0, dungeonWidth), 0);
            }else{ // Cote droit
                _bossRoom = (Random.Range(0, dungeonWidth), dungeonLength - 1);
            }
        }else{ // En face (haut)
            _bossRoom = (0, Random.Range(0, dungeonLength));
        }
        _dungeonGrid[_bossRoom.Item1][_bossRoom.Item2].RoomType = 9;
    }

    bool ChoosePath(int x, int z){ // Choix automatique du chemin a suivre pour aller a la salle du boss
        if((x, z) == _bossRoom){
            _actualPath.Add((x, z));
            return true;
        }else{
            List<(int, int)> neighboursList = GetNeighbours(x, z);
            
            foreach((int, int) neighbour in neighboursList.ToArray()){
                if(neighbour == _bossRoom){
                    _actualPath.Add(_bossRoom);
                    _dungeonGrid[x][z].RoomType = 2;
                    _actualPath.Add((x, z));
                    return true;
                }
                if(_dungeonGrid[neighbour.Item1][neighbour.Item2].RoomType == 0){
                    _dungeonGrid[neighbour.Item1][neighbour.Item2].RoomType = 1;
                }else{
                    neighboursList.Remove(neighbour);
                }
            }

            if(neighboursList.Count != 0){
                foreach((int, int) neighbour in ShuffleList(neighboursList)){
                    if(ChoosePath(neighbour.Item1, neighbour.Item2)){
                        _dungeonGrid[x][z].RoomType = 2;
                        _actualPath.Add((x, z));
                        return true;
                    }
                }
            }

            return false;
        }
    }

    List<(int, int)> GetNeighbours(int x, int z){ // Obtention de la liste des voisins
        List<(int, int)> neighboursList = new List<(int, int)>();

        if(x > 0 && x < dungeonWidth - 1){ // Pas colle a un cote vertical
            if(z > 0 && z < dungeonLength - 1){ // Pas colle a un cote horizontal
                neighboursList.Add((x-1, z)); // Haut
                neighboursList.Add((x, z-1)); // Gauche
                neighboursList.Add((x, z+1)); // Droite
                neighboursList.Add((x+1, z)); // Bas
            }else{ // Colle a un cote horizontal
                if(z < dungeonLength - 1){ // Colle a gauche
                    neighboursList.Add((x-1, z)); // Haut
                    neighboursList.Add((x, z+1)); // Droite
                    neighboursList.Add((x+1, z)); // Bas
                }else{ // Colle a droite
                    neighboursList.Add((x-1, z)); // Haut
                    neighboursList.Add((x, z-1)); // Gauche
                    neighboursList.Add((x+1, z)); // Bas
                }
            }
        }else{ // Colle a un cote vertical
            if(x < dungeonWidth - 1){ // Colle en haut
                if(z > 0 && z < dungeonLength - 1){ // Pas colle a un cote horizontal
                    neighboursList.Add((x, z-1)); // Gauche
                    neighboursList.Add((x, z+1)); // Droite
                    neighboursList.Add((x+1, z)); // Bas
                }else{ // Colle a un cote horizontal
                    if(z < dungeonLength - 1){ // Colle a gauche
                        neighboursList.Add((x, z+1)); // Droite
                        neighboursList.Add((x+1, z)); // Bas
                    }else{ // Colle a droite
                        neighboursList.Add((x, z-1)); // Gauche
                        neighboursList.Add((x+1, z)); // Bas
                    }
                }
            }else{ // Colle en bas
                if(z > 0 && z < dungeonLength - 1){ // Pas colle a un cote horizontal
                    neighboursList.Add((x, z-1)); // Gauche
                    neighboursList.Add((x, z+1)); // Droite
                    neighboursList.Add((x-1, z)); // Haut
                }else{ // Colle a un cote horizontal
                    if(z < dungeonLength - 1){ // Colle a gauche
                        neighboursList.Add((x, z+1)); // Droite
                        neighboursList.Add((x-1, z)); // Haut
                    }else{ // Colle a droite
                        neighboursList.Add((x, z-1)); // Gauche
                        neighboursList.Add((x-1, z)); // Haut
                    }
                }
            }
        }

        return neighboursList;
    }

    void AddDoorsInformations(){ // Ajoute les informations a propos des portes de la salle
        for(int z = 0; z < dungeonWidth; z++){
            for(int x = 0; x < dungeonLength; x++){
                if(_dungeonGrid[z][x].RoomType == 2 || _dungeonGrid[z][x].RoomType == 9){
                    List<int> doorsLocation = FindDoors(x, z); // Cherche les portes
                    _dungeonGrid[z][x].DoorsLocations = doorsLocation;
                }
            }
        }
    }

    List<int> FindDoors(int x, int z){ // Trouve ou se situent les portes de la salle
        List<int> doorsList = new List<int>();
        List<(int, int)> neighboursList = GetNeighbours(z, x);
        foreach((int, int) neighbour in neighboursList){
            if(_dungeonGrid[neighbour.Item1][neighbour.Item2].RoomType == 2 || _dungeonGrid[neighbour.Item1][neighbour.Item2].RoomType == 9){
                if(neighbour.Item2 > x){ // Droite
                    doorsList.Add(4);
                }
                if(neighbour.Item2 < x){ // Gauche
                    doorsList.Add(1);
                }
                if(neighbour.Item1 > z){ // Bas
                    doorsList.Add(3);
                }
                if(neighbour.Item1 < z){ // Haut
                    doorsList.Add(2);
                }
            }
        }
        return doorsList;
    }

    void CreateInGame(){
        foreach(Transform child in floorParent){
            Destroy(child.gameObject);
        }
        foreach(Transform child in doorParent){
            Destroy(child.gameObject);
        }
        foreach(Transform child in wallParent){
            Destroy(child.gameObject);
        }
        for(int z = 0; z < dungeonWidth; z++){
            for(int x = 0; x < dungeonLength; x++){
                if(_dungeonGrid[z][x].RoomType != 0){

                    if(_dungeonGrid[z][x].RoomType != 1){
                        GameObject newFloor = Instantiate(floor, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                        newFloor.transform.SetParent(floorParent);

                        // Affichage couleur temporaire
                        
                        if(_dungeonGrid[z][x].RoomType == 2){
                            newFloor.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.white;
                        }
                        if(_dungeonGrid[z][x].RoomType == 4){
                            newFloor.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = new Color(0.5f, 0f, 1f, 1f);
                        }
                        if(_dungeonGrid[z][x].RoomType == 5){
                            newFloor.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;
                        }
                        if(_dungeonGrid[z][x].RoomType == 6){
                            newFloor.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.green;
                        }
                        if(_dungeonGrid[z][x].RoomType == 9){
                            newFloor.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                        }
                    }

                    // Ajout murs et portes

                    if(_dungeonGrid[z][x].DoorsLocations.Contains(2)){ // Haut (porte)
                        GameObject newDoor = Instantiate(door, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                        newDoor.transform.position += new Vector3(0, 0, 8);
                        newDoor.transform.SetParent(doorParent);
                    }else{ // Mur
                        if(_dungeonGrid[z][x].RoomType == 1){
                            if(z != 0){
                                if(_dungeonGrid[z - 1][x].RoomType != 0 && _dungeonGrid[z - 1][x].RoomType != 1){
                                    GameObject newWall = Instantiate(wall, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                                    newWall.transform.position += new Vector3(0, 0, 8);
                                    newWall.transform.SetParent(wallParent);
                                }
                            }
                        }else{
                            GameObject newWall = Instantiate(wall, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                            newWall.transform.position += new Vector3(0, 0, 8);
                            newWall.transform.SetParent(wallParent);
                        }
                    }
                    if(_dungeonGrid[z][x].DoorsLocations.Contains(4)){ // Droite (porte)
                        GameObject newDoor = Instantiate(door, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                        newDoor.transform.position += new Vector3(8, 0, 0);
                        newDoor.transform.eulerAngles += new Vector3(0, 90, 0);
                        newDoor.transform.SetParent(doorParent);
                    }else{ // Mur
                        if(_dungeonGrid[z][x].RoomType == 1){
                            if(x != dungeonLength - 1){
                                if(_dungeonGrid[z][x + 1].RoomType != 0 && _dungeonGrid[z][x + 1].RoomType != 1){
                                    GameObject newWall = Instantiate(wall, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                                    newWall.transform.position += new Vector3(8, 0, 0);
                                    newWall.transform.eulerAngles += new Vector3(0, 90, 0);
                                    newWall.transform.SetParent(wallParent);
                                }
                            }
                        }else{
                            GameObject newWall = Instantiate(wall, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                            newWall.transform.position += new Vector3(8, 0, 0);
                            newWall.transform.eulerAngles += new Vector3(0, 90, 0);
                            newWall.transform.SetParent(wallParent);
                        }
                    }

                    if(x == 0 && _dungeonGrid[z][x].RoomType != 1){ // Murs a gauche
                        GameObject newWall = Instantiate(wall, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                        newWall.transform.position += new Vector3(-8, 0, 0);
                        newWall.transform.eulerAngles += new Vector3(0, 90, 0);
                        newWall.transform.SetParent(wallParent);
                    } // Murs en bas
                    if(z == dungeonWidth - 1 && _startRoom != (z, x) && _dungeonGrid[z][x].RoomType != 1){ // Murs en bas
                        GameObject newWall = Instantiate(wall, new Vector3(x * 16 - _startRoom.Item2 * 16, 0, (dungeonWidth - (z+1)) * 16 + 16), Quaternion.identity);
                        newWall.transform.position += new Vector3(0, 0, -8);
                        newWall.transform.SetParent(wallParent);
                    }
                }
            }
        }
    }
    void ShowGrid(){ // Sert a afficher la grille
        string gridTxt = "DungeonGrid : \n\n\t";
        for(int z = 0; z < dungeonWidth; z++){
            for(int x = 0; x < dungeonLength; x++){
                gridTxt += $" {_dungeonGrid[z][x].RoomType} ";
            }
            gridTxt += "\n\t";
        }
        Debug.Log(gridTxt);
    }

    #endregion

}
