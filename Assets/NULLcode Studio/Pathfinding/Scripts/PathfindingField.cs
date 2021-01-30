using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingField : MonoBehaviour
{

    public Color defaultColor; // цвет клетки по умолчанию
    public Color pathColor; // подсветка пути
    public Color pathMayToWay; // подсветка возможного пути
    public Color cursorColor; // подсветка указателя
    public Color mayBlowColor; // подсветка возможного взрыва
    public Color mayToThrowBlowColor; // подсветка возмжного длины броска
    public Color shootRateColor; // подсветка возмжного длины выстрела
    public Color shootColor; // подсветка возмжоного выстрела
    public Color enemyColor; // подсветка врага
    public Color integratColor; // подсветка интегрируемого квадарата
    public LayerMask layerMask; // маска клетки
    public int width;
    public int height;
    [SerializeField] [Range(1f, 10f)] private float moveSpeed = 1;
    //[SerializeField] [Range(0.1f, 1f)] private float rotationSpeed = 0.25f;
    [SerializeField] private PathfindingNode[] grid;
    private List<PathfindingNode> path;
    private List<TargetPath> pathTarget;
    private PathfindingNode start, end, last;
    private Transform target;
    private int hash, index;
    private bool move;
    private PathfindingNode[,] map;
    private static PathfindingField _inst;

    [HideInInspector] public TakeInfoToList infoAgent;
    [HideInInspector] public TakeInfoToListE infoAgentE;
    [HideInInspector] public PlaceCordToQuads manageQuads;

    public InfoUnit infoU;
    public bool showThrowDistance;
    public GameObject canvasPB;
    public GameObject canvasPBE;
    public GameObject SphereFoW;
    public GameObject textInfo;
    GameObject IntegrObj;

    bool switched = false;
    void Awake()
    {
        _inst = this;
        BuildMap();
        infoAgent = GameObject.FindGameObjectWithTag("Controls").gameObject.GetComponent<TakeInfoToList>();
        infoAgentE = GameObject.FindGameObjectWithTag("EControls").gameObject.GetComponent<TakeInfoToListE>();
        manageQuads = gameObject.GetComponent<PlaceCordToQuads>();
        IntegrObj = GameObject.FindGameObjectWithTag("IntegrationsObj").gameObject;
    }

    void Start()
    {
    }

    // обновление состояния клеток поля, эту функцию нужно вызывать, если на поле появляются новые объекты или уничтожаются имеющиеся
    public static void UpdateNodeState()
    {
        _inst.UpdateNodeState_inst();
    }

    void BuildMap() // инициализация двумерного массива
    {
        map = new PathfindingNode[width, height];
        int i = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[i].x = x;
                grid[i].y = y;
                map[x, y] = grid[i];
                i++;
            }
        }

        UpdateNodeState_inst();
    }

    Vector3 NormalizeVector(Vector3 val) // округляем вектор до сотых
    {
        val.x = Mathf.Round(val.x * 100f) / 100f;
        val.y = Mathf.Round(val.y * 100f) / 100f;
        val.z = Mathf.Round(val.z * 100f) / 100f;
        return val;
    }

    struct TargetPath
    {
        public Vector3 direction, position;
    }

    struct EnemyParamToAttack
    {
        public int chanseTohit;
        public int shotThroughtToEnemy;
        public StatusEnemy enemy;
    }
    EnemyParamToAttack enemy1;

    void BuildUnitPath() // создание точек движения и вектора вращения для юнита, на основе найденного пути
    {
        if (infoAgent.GetChosenAgent().status == 1)
        {
            TargetPath p = new TargetPath();
            pathTarget = new List<TargetPath>();
            Vector3 directionLast = (path[0].transform.position - target.position).normalized;


            p.direction = directionLast;
            p.position = target.position;
            pathTarget.Add(p);

            if (end.target != null) path.RemoveAt(path.Count - 1);

            for (int i = 0; i < path.Count; i++)
            {
                int id = (i + 1 < path.Count - 1) ? i + 1 : path.Count - 1;
                Vector3 direction = (path[id].transform.position - path[i].transform.position).normalized;

                if (direction != directionLast)
                {
                    if (!path[i].enemyStayingHere) // Если на пути стоит враг, то это конец пути прохождения
                    {
                        p = new TargetPath();
                        p.direction = (i < path.Count - 1) ? direction : directionLast;
                        p.position = NormalizeVector(path[i].transform.position);
                        pathTarget.Add(p);
                    }
                    else
                        break;
                }

                directionLast = direction;
            }

            //manageQuads.ClearMap();

        }
    }

    void UpdateMove()
    {
        if (!move) return;

        //if(NormalizeVector(pathTarget[index].direction) != NormalizeVector(target.forward))
        //{
        //	// анимация поворота юнита по вектору движения
        //	target.rotation = Quaternion.Lerp(target.rotation, Quaternion.LookRotation(pathTarget[index].direction), rotationSpeed);
        //}
        /*else*/
        if (index < pathTarget.Count - 1)
        {
            // анимация движения к следующей точке
            Vector3 pathTarPos = pathTarget[index + 1].position;
            pathTarPos.y = 0.4f;
            target.position = Vector3.MoveTowards(target.position, pathTarPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(target.position, pathTarPos) < 0.1f)
            {
                target.position = pathTarPos;
                index++;
            }
        }
        else if (pathTarget.Count > 0 && index == pathTarget.Count - 1 && end.target != null && start.isPlayer)
        {
            // если юнита направили на другого юнита, когда он дойдет до него
            // добавляем еще одну точку с текущей позицией и новым направлением, чтобы юнит развернулся "носом" к цели
            TargetPath p = new TargetPath();
            p.direction = (end.target.position - target.position).normalized;
            p.position = target.position;
            pathTarget.Add(p);
            start.isPlayer = false;
            index++;
        }
        else // если юнит достиг цели
        {
            if (end.enemyStayingHere)
            {
                infoAgent.FindEnemyOnNode(end.idNode).GetComponent<StatusEnemy>().hp -= 20;
            }

            UpdateNodeState_inst();
            //start.isPlayer = false;
            start = null;
            end = null;
            hash = 0;
            move = false;

            infoAgent.GetChosenAgent().moved = false;
            infoAgent.GetChosenAgent().isMoved = true;
            infoAgent.SetStatusAgent(0);
            PathfindingNode node = infoAgent.FindNodeOnPlayer(infoAgent.GetChosenAgent().iAmStayOnNode).GetComponent<PathfindingNode>();
            SetChosenAgentPF(ref node, infoAgent.GetChosenAgent().gameObject);
            //canvasPB.transform.SetParent(null);
            //infoAgent.GetChosenAgent().hasChosen = false;
            //infoU.SelectedUnitName("");
            
            manageQuads.ClearMap();

        }
    }

    void LateUpdate()
    {
        UpdateMove();
        if (move) return;

        if (!infoAgent.isHaveChosenAgentBool())
            canvasPB.SetActive(false);
        else
            PlaceBarToChosenAgent();

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            PathfindingNode node = hit.transform.GetComponent<PathfindingNode>();
            GameObject agent = infoAgent.FindPlayerOnNode(node.idNode);

            //if (node == null || node.isLock && node.target == null) return;

            if (Input.GetMouseButtonDown(0))
            { //	node.target != null

                if (infoAgent.FindPlayerOnNode(node.idNode) != null && end == null)
                {
                    UpdateNodeState_inst();
                    SetChosenAgentPF(ref node, agent);
                }
                else if (infoAgent.isHaveChosenAgentBool() && infoAgent.GetChosenAgent().status == 2)
                {
                    // Обработка Attack Greanade
                    for (int i = 0; i < grid.Length; i++)
                    {
                        if (grid[i].blowQuad && infoAgentE.IsEnemyStayingNode(grid[i].idNode))
                            infoAgent.FindEnemyOnNode(grid[i].idNode).gameObject.GetComponent<StatusEnemy>().hp -= 30;
                        else if (infoAgent.IsPlayerStayingNode(grid[i].idNode) && grid[i].blowQuad)
                            infoAgent.FindPlayerOnNode(grid[i].idNode).gameObject.GetComponent<StatusPlayer>().hp -= 30;
                    }
                    infoAgent.GetChosenAgent().isThrowG = true;
                    if (infoAgent.GetChosenAgent().isMoved == false)
                        infoAgent.SetStatusAgent(1);
                    else
                        infoAgent.SetStatusAgent(0);
                    infoAgent.UpdateList();
                    infoAgentE.UpdateList();
                    manageQuads.ClearMap();
                    FieldUpdate();
                }
                else if (node.enemyStayingHere)
                {
                    if(infoAgent.GetChosenAgent().status != 0)
                    if(infoAgent.GetChosenAgent().status == 1)
                    {
                        target = start.target;
                        BuildUnitPath();
                        index = 0;
                        move = true;

                        infoAgent.GetChosenAgent().moved = true;
                    }
                    else if (infoAgent.GetChosenAgent().status == 4)
                    {
                        // Обработка Use Ability

                        infoAgent.GetChosenAgent().isUseAbility = true;
                        if(infoAgent.GetChosenAgent().isMoved == false)
                            infoAgent.SetStatusAgent(1);
                        else
                            infoAgent.SetStatusAgent(0);

                        manageQuads.ClearMap();
                        FieldUpdate();

                    }
                }
                else if (end != null && end.iAmTagged && end.isFoVActiveNode)
                {
                    if (infoAgent.GetChosenAgent().status == 1)
                    {
                        target = start.target;
                        BuildUnitPath();
                        index = 0;
                        move = true;

                        infoAgent.GetChosenAgent().moved = true;
                    }
                }

            }
            else if (Input.GetMouseButtonDown(1) && start != null)
            {
                start.isPlayer = false;
                FieldUpdate();
                start = null;
                end = null;
                hash = 0;

                infoU.SelectedUnitName("");
                canvasPB.transform.SetParent(null);
                canvasPB.SetActive(false);
                infoAgent.GetChosenAgent().hasChosen = false;
            }

            if (hash != node.GetInstanceID())
            {
                if (last != null && !last.isPlayer && last.isActivatedNode) last.mesh.material.color = defaultColor;
                if (!node.isPlayer && node.isActivatedNode) node.mesh.material.color = cursorColor;
                else if (node.isIntegrationsNode) node.mesh.material.color = integratColor;

                if (start != null && end != null)
                {
                    FieldUpdate();
                    end = null;
                }

                if (start != null && node != null)
                {
                    if (infoAgent.isHaveChosenAgentBool())
                        if (infoAgent.GetChosenAgent().status == 1)
                        {
                            if (!infoAgent.GetChosenAgent().isMoved)
                            {
                                if (node.iAmTagged)
                                UpdateMap(node);

                                for (int j = 0; j < grid.Length; j++)
                                {
                                    if (grid[j].isActivatedNode )
                                    {
                                        if (grid[j].iAmTagged && grid[j].isFoVActiveNode)
                                        {
                                            if (grid[j].enemyStayingHere)
                                                grid[j].mesh.material.color = enemyColor;
                                            else
                                                grid[j].mesh.material.color = pathMayToWay;
                                        }

                                    }
                                    else
                                        grid[j].mesh.material.color = Color.clear;
                                }

                                for (int i = 0; i < path.Count; i++)
                                {
                                    if (path[i].isActivatedNode)
                                    {
                                        if (path[i].iAmTagged && path[i].isFoVActiveNode)
                                        {
                                            path[i].mesh.material.color = pathColor;
                                        }
                                    }
                                    else
                                        path[i].mesh.material.color = Color.clear;
                                }
                            }
                        }
                        else if (infoAgent.GetChosenAgent().status == 2)
                        {
                            if (!infoAgent.GetChosenAgent().isThrowG)
                            {
                                UpdateMap(node);

                                for (int j = 0; j < grid.Length; j++)
                                {
                                    if (grid[j].isActivatedNode)
                                    {
                                        if (showThrowDistance)
                                        {
                                            if (grid[j].iAmTagged)
                                                grid[j].mesh.material.color = mayToThrowBlowColor;
                                            else
                                                grid[j].mesh.material.color = defaultColor;
                                        }

                                        if (showThrowDistance == false)
                                            grid[j].mesh.material.color = defaultColor;

                                        if (grid[j].blowQuad)
                                            grid[j].mesh.material.color = mayBlowColor;
                                        else if (grid[j].enemyStayingHere)
                                            grid[j].mesh.material.color = enemyColor;
                                    }
                                    else
                                        grid[j].mesh.material.color = Color.clear;

                                }
                            }
                        }
                        else if (infoAgent.GetChosenAgent().status == 3)
                        {
                            if (!infoAgent.GetChosenAgent().isShooted)
                            {
                                UpdateMap(node);

                                for (int j = 0; j < grid.Length; j++)
                                {
                                    if (grid[j].isActivatedNode)
                                    {
                                        if (grid[j].iAmTagged)
                                        {
                                            if (grid[j].shootQuad)
                                                grid[j].mesh.material.color = shootColor;
                                            else if (grid[j].enemyStayingHere)
                                                grid[j].mesh.material.color = enemyColor;
                                            else if (grid[j].iAmTagged)
                                                grid[j].mesh.material.color = shootRateColor;
                                        }
                                    }
                                    else
                                        grid[j].mesh.material.color = Color.clear;

                                }
                            }
                        }

                }
            }



            if (infoAgent.isHaveChosenAgentBool())
            {
                if (infoAgent.GetChosenAgent().status == 1)
                {
                    if (!infoAgent.GetChosenAgent().isMoved)
                    {
                        // код для 1 состояния Агента

                    }
                }
                else if (infoAgent.GetChosenAgent().status == 2)
                {

                }
                else if (infoAgent.GetChosenAgent().status == 3)
                {
                    if (!infoAgent.GetChosenAgent().isShooted && node.iAmTagged)
                    {

                        if (!node.nodeOnFog)
                        {
                            if (infoAgent.FindEnemyOnNode(node.idNode) != null)
                            {

                                StatusEnemy enemy = infoAgent.FindEnemyOnNode(node.idNode).GetComponent<StatusEnemy>();
                                textInfo.transform.GetChild(0).GetComponent<Text>().text = enemy.name;
                                int shotThroughtCount = 0;
                                enemy1.enemy = enemy;

                                Vector3 hitedPos = enemy.gameObject.transform.position, startPos = infoAgent.GetChosenAgent().transform.position;
                                float dist = Vector3.Distance(startPos, hitedPos);
                                textInfo.transform.GetChild(3).GetComponent<Text>().text = dist.ToString();
                                startPos.y += .15f;
                                hitedPos.y = startPos.y;

                                ShotCalculate(startPos, hitedPos, shotThroughtCount, dist);
                            }
                            else
                            {
                                textInfo.transform.GetChild(0).GetComponent<Text>().text = "";
                                textInfo.transform.GetChild(1).GetComponent<Text>().text = "";
                                textInfo.transform.GetChild(2).GetComponent<Text>().text = "";
                                textInfo.transform.GetChild(3).GetComponent<Text>().text = "";
                                textInfo.transform.GetChild(4).GetComponent<Text>().text = "";
                                textInfo.transform.GetChild(5).GetComponent<Text>().text = "";

                            }
                        }
                    }

                }

            }
            else
            {
                manageQuads.ClearMap();

            }


            //print(target);
            //print(node.target);

            last = node;
            hash = node.GetInstanceID();


        }

        if (IntegrObj != null)
            for (int i = 0; i < IntegrObj.transform.childCount; i++)
            {
                IntegrationsObj interObj = IntegrObj.transform.GetChild(i).GetComponent<IntegrationsObj>();
                if (infoAgent.isHaveChosenAgentBool())
                    for (int j = 0; j < interObj.idNodeOnStaying.Count; j++)
                        if (infoAgent.GetChosenAgent().iAmStayOnNode == interObj.idNodeOnStaying[j])
                            if (Input.GetKeyDown(KeyCode.O))
                                interObj.OpensDoor();
            }
    }


    void UpdateMap(PathfindingNode node)
    {
        end = node;

        path = Pathfinding.Find(start, node, map, width, height);

        if (path == null)
        {
            FieldUpdate();
            start = null;
            end = null;
            hash = 0;
            return;
        }
        FieldUpdate();
    }

    void UpdateNodeState_inst() // обновления поля, после совершения действия
    {
        for (int i = 0; i < grid.Length; i++)
        {
            RaycastHit hit; // пускаем луч сверху на клетку, проверяем занята она или нет
            Physics.Raycast(grid[i].transform.position + Vector3.up * 100f, Vector3.down, out hit, 100f, ~layerMask);

            if (hit.collider == null) // пустая клетка
            {
                grid[i].target = null;
                grid[i].isLock = false;
                grid[i].isPlayer = false;
                grid[i].cost = -1; // свободное место
            }
            else if (hit.collider.tag == "Enemy")
            {
                grid[i].isLock = true;
                grid[i].isPlayer = false;
                grid[i].cost = -2;
            }
            else if (hit.collider.tag == "Player") // найден юнит
            {
                grid[i].target = hit.transform;
                grid[i].isLock = true;
                grid[i].isPlayer = false;
                grid[i].cost = -2; // препятствие
            }
            else if (hit.collider.tag == "CapObstacle")
            {
                grid[i].isLock = true;
                grid[i].cost = -2;
            }
            else if (hit.collider.tag == "TriggerRangeOfView")
            {
                grid[i].target = null;
                grid[i].isLock = false;
                grid[i].isPlayer = false;
                grid[i].cost = -1; // свободное место
            }
            else // любой другой объект/препятствие
            {
                grid[i].isLock = true;
                grid[i].cost = -2;
            }

            grid[i].mesh.material.color = defaultColor;
        }
    }

    public void FieldUpdate() // обновление поля, перед подсветкой пути
    {
        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i].isPlayer)
            {
                grid[i].cost = -1;
            }
            else if (grid[i].isLock)
            {
                grid[i].cost = -2;
                grid[i].mesh.material.color = defaultColor;
            }
            else if (grid[i].isIntegrationsNode)
            {
                grid[i].mesh.material.color = integratColor;
                grid[i].cost = -1;
            }
            else
            {
                grid[i].mesh.material.color = defaultColor;
                grid[i].cost = -1;
            }
        }
    }

    /// Используются в кнопках
    public void GeneratePathMayToWay()
    {
        if (infoAgent.isHaveChosenAgentBool())
        {
            infoAgent.GetChosenAgent().transform.GetChild(0).GetComponent<SphereCollider>().radius = 12f;
            infoAgent.GetChosenAgent().transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void GeneratePlaceMayToBlow()
    {
        if (infoAgent.isHaveChosenAgentBool())
        {
            infoAgent.GetChosenAgent().transform.GetChild(0).GetComponent<SphereCollider>().radius = 26f;
            infoAgent.GetChosenAgent().transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void GeneratePlaceShootRate()
    {
        if (infoAgent.isHaveChosenAgentBool())
        {
            infoAgent.GetChosenAgent().transform.GetChild(0).GetComponent<SphereCollider>().radius = 30f;
            infoAgent.GetChosenAgent().transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    /// Перемещение полоски ХП к выбранному агенту
    void PlaceBarToChosenAgent()
    {
        if (infoAgent.isHaveChosenAgentBool())
        {
            if (switched == false)
            {
                canvasPB.SetActive(true);
                canvasPB.transform.SetParent(infoAgent.GetChosenAgent().transform);
                canvasPB.GetComponent<RectTransform>().localPosition = new Vector3(-0.3f, 2.3f, 0f);
                infoAgent.GetChosenAgent().GetComponent<StatusPlayer>().healthBar = canvasPB.transform.GetChild(0).GetComponent<HP>();
            }
            else
            {
                canvasPB.SetActive(true);
                canvasPB.transform.SetParent(infoAgent.GetChosenAgent().transform);
                canvasPB.GetComponent<RectTransform>().localPosition = new Vector3(-0.3f, -2.3f, 0f);
                infoAgent.GetChosenAgent().GetComponent<StatusPlayer>().healthBar = canvasPB.transform.GetChild(0).GetComponent<HP>();
            }
        }
    }

    /// Выбор/Смена агента ( Сделан чтобы укоротить код в середине )
    void SetChoseAgent(GameObject agent)
    {
        if (!infoAgent.isHaveChosenAgentBool())
        {
            agent.GetComponent<StatusPlayer>().hasChosen = true;
            if (!infoAgent.GetChosenAgent().isMoved)
            {
                infoU.SelectedUnitName(agent.GetComponent<StatusPlayer>().nameAgent);
                infoAgent.SetStatusAgent(1);
                manageQuads.ClearMap();
                GeneratePathMayToWay();
            }
            else
            {
                infoU.SelectedUnitName(agent.GetComponent<StatusPlayer>().nameAgent);
                infoAgent.SetStatusAgent(0);
                manageQuads.ClearMap();
            }
        }
        else
        {
            infoAgent.FindNodeOnPlayer(infoAgent.GetChosenAgent().iAmStayOnNode).GetComponent<PathfindingNode>().mesh.material.color = defaultColor;
            infoAgent.GetChosenAgent().hasChosen = false;
            agent.GetComponent<StatusPlayer>().hasChosen = true;
            if (!infoAgent.GetChosenAgent().isMoved)
            {
                infoU.SelectedUnitName(agent.GetComponent<StatusPlayer>().nameAgent);
                infoAgent.SetStatusAgent(1);
                manageQuads.ClearMap();
                GeneratePathMayToWay();
            }
            else
            {
                infoU.SelectedUnitName(agent.GetComponent<StatusPlayer>().nameAgent);
                infoAgent.SetStatusAgent(0);
                manageQuads.ClearMap();
            }
        }
    }

    /// Смена управления
    public void SwitchControls()
    {
        if (infoAgent.isHaveChosenAgentBool())
        {
            infoAgent.GetChosenAgent().status = 0;
            canvasPB.transform.SetParent(null);
            infoAgent.GetChosenAgent().hasChosen = false;
        }

        List<StatusPlayer> bufSP = new List<StatusPlayer>();
        List<StatusEnemy> bufSE = new List<StatusEnemy>();
        
        for (int i = 0; i < infoAgent.Agents.Count; i++)
        {
            infoAgent.Agents[i].GetComponent<FieldOfView>().enabled = false;
            bufSP.Add(infoAgent.Agents[i]);
            Destroy(infoAgent.Agents[i].GetComponent<StatusPlayer>());
            infoAgent.Agents[i].gameObject.AddComponent<StatusEnemy>();
            infoAgent.Agents[i].gameObject.transform.SetParent(infoAgentE.transform);
        }
        for (int i = 0; i < infoAgentE.Agents.Count; i++)
        {
            infoAgentE.Agents[i].GetComponent<FieldOfView>().enabled = false;
            bufSE.Add(infoAgentE.Agents[i]);
            Destroy(infoAgentE.Agents[i].GetComponent<StatusEnemy>());
            infoAgentE.Agents[i].gameObject.AddComponent<StatusPlayer>();
            infoAgentE.Agents[i].gameObject.transform.SetParent(infoAgent.transform);
        }

        infoAgent.Agents.Clear();
        infoAgentE.Agents.Clear();

        infoAgent.UpdateList();
        infoAgentE.UpdateList();

        for (int i = 0; i < infoAgentE.Agents.Count; i++)
        {
            GameObject instAgent = infoAgentE.Agents[i].gameObject;
            instAgent.GetComponent<StatusEnemy>().nameAgent = bufSP[i].nameAgent;
            instAgent.GetComponent<StatusEnemy>().status = bufSP[i].status;
            instAgent.GetComponent<StatusEnemy>().maxhp = bufSP[i].maxhp;
            instAgent.GetComponent<StatusEnemy>().hp = bufSP[i].hp;
            instAgent.GetComponent<StatusEnemy>().arm = bufSP[i].arm;
            instAgent.GetComponent<StatusEnemy>().iAmStayOnNode = bufSP[i].iAmStayOnNode;
            instAgent.GetComponent<StatusEnemy>().idGun = bufSP[i].idGun;

            instAgent.GetComponent<StatusEnemy>().hurted = bufSP[i].hurted;
            instAgent.GetComponent<StatusEnemy>().isMidfield = bufSP[i].isMidfield;
            instAgent.GetComponent<StatusEnemy>().isFProt = bufSP[i].isFProt;
            instAgent.GetComponent<StatusEnemy>().idGun = bufSP[i].idGun;

            instAgent.GetComponent<StatusEnemy>().isMoved = bufSP[i].isMoved;
            instAgent.GetComponent<StatusEnemy>().isThrowG = bufSP[i].isThrowG;
            instAgent.GetComponent<StatusEnemy>().isShooted = bufSP[i].isShooted;
            instAgent.GetComponent<StatusEnemy>().isUseAbility = bufSP[i].isUseAbility;
            instAgent.transform.SetParent(infoAgentE.transform);

            GameObject instCanvas = Instantiate(canvasPBE, instAgent.transform);
            if (switched == false)
            {
                instCanvas.GetComponent<RectTransform>().localPosition = new Vector3(-0.3f, 2.3f, 0f);
                instCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.0043f, 0.0043f, 0.0043f);
                instAgent.GetComponent<StatusEnemy>().healthBar = instCanvas.transform.GetChild(0).GetComponent<HP>();
            }
            else
            {
                instCanvas.GetComponent<RectTransform>().localPosition = new Vector3(-0.3f, -2.3f, 0f);
                instCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.0043f, 0.0043f, 0.0043f);
                instAgent.GetComponent<StatusEnemy>().healthBar = instCanvas.transform.GetChild(0).GetComponent<HP>();
            }
            instAgent.GetComponent<FieldOfView>().enabled = false;
            instAgent.GetComponent<MeshRenderer>().enabled = false;

            for (int i1 = 0; i1 < instAgent.transform.childCount; i1++)
                if (instAgent.transform.GetChild(i1).GetComponent<FieldOfView>())
                    instAgent.transform.GetChild(i1).GetComponent<FieldOfView>().enabled = false;


            instAgent.layer = 11; // Layer Enemy
            instAgent.tag = "Enemy";
        }

        for (int i = 0; i < infoAgent.Agents.Count; i++)
        {
            GameObject instAgent = infoAgent.Agents[i].gameObject;

            instAgent.GetComponent<StatusPlayer>().nameAgent = bufSE[i].nameAgent;
            instAgent.GetComponent<StatusPlayer>().status = bufSE[i].status;
            instAgent.GetComponent<StatusPlayer>().maxhp = bufSE[i].maxhp;
            instAgent.GetComponent<StatusPlayer>().hp = bufSE[i].hp;
            instAgent.GetComponent<StatusPlayer>().arm = bufSE[i].arm;
            instAgent.GetComponent<StatusPlayer>().iAmStayOnNode = bufSE[i].iAmStayOnNode;
            instAgent.GetComponent<StatusPlayer>().idGun = bufSE[i].idGun;

            instAgent.GetComponent<StatusPlayer>().hurted = bufSE[i].hurted;
            instAgent.GetComponent<StatusPlayer>().isMidfield = bufSE[i].isMidfield;
            instAgent.GetComponent<StatusPlayer>().isFProt = bufSE[i].isFProt;
            instAgent.GetComponent<StatusPlayer>().idGun = bufSE[i].idGun;

            instAgent.GetComponent<StatusPlayer>().isMoved = bufSE[i].isMoved;
            instAgent.GetComponent<StatusPlayer>().isThrowG = bufSE[i].isThrowG;
            instAgent.GetComponent<StatusPlayer>().isShooted = bufSE[i].isShooted;
            instAgent.GetComponent<StatusPlayer>().isUseAbility = bufSE[i].isUseAbility;
            instAgent.transform.SetParent(infoAgent.transform);

            for (int i1 = 0; i1 < instAgent.transform.childCount; i1++)
                if (instAgent.transform.GetChild(i1).GetComponent<FieldOfView>())
                {
                    instAgent.transform.GetChild(i1).GetComponent<FieldOfView>().gameObject.SetActive(true);
                    instAgent.transform.GetChild(i1).GetComponent<FieldOfView>().enabled = true;
                }
                else if (instAgent.transform.GetChild(i1).GetComponent<DelayToOffCollider>())
                    instAgent.transform.GetChild(i1).GetComponent<DelayToOffCollider>().enabled = true;
                else if (instAgent.transform.GetChild(i1).gameObject.layer == 5)
                    Destroy(instAgent.transform.GetChild(i1).gameObject);

            instAgent.GetComponent<FieldOfView>().enabled = true;
            instAgent.GetComponent<MeshRenderer>().enabled = true;
            instAgent.layer = 8; // Layer Player
            instAgent.tag = "Player";
        }

        manageQuads.ClearMap();
        FieldUpdate();
        UpdateNodeState();
        infoAgent.UpdateList();
        infoAgentE.UpdateList();

        if (switched) switched = false;
        else switched = true;

        bufSP.Clear();
        bufSE.Clear();
    }

    void ShotCalculate(Vector3 ObjPos,Vector3 hitedPos, int shotThroughtCount,float distToEnemy)
    {
        RaycastHit lineHit;
        // Вместо linecast использовать rayCast, дабы избежать не нужной игроком цели

        Debug.DrawLine(ObjPos, hitedPos);
        if (Physics.Linecast(ObjPos, hitedPos, out lineHit))
        {
            //print(lineHit.collider.name);
            if (lineHit.transform.gameObject.GetComponent<ToggleCaptionMesh>())
            {
                    if (lineHit.transform.gameObject.GetComponent<ToggleCaptionMesh>().isShotThrough)
                    {
                        Vector3 hitedObjPos = lineHit.transform.gameObject.transform.position;
                        hitedObjPos.x = hitedPos.x;
                        hitedObjPos.y = hitedPos.y;

                        shotThroughtCount++;
                        ShotCalculate(hitedObjPos, hitedPos, shotThroughtCount, distToEnemy);
                    }
            }
            else if (lineHit.transform.gameObject.GetComponent<StatusEnemy>() == enemy1.enemy)
            {
                textInfo.transform.GetChild(4).GetComponent<Text>().text = shotThroughtCount.ToString();
                int chanseToHit = 0;
                if (distToEnemy <= 3f)
                    chanseToHit = calcChanse(infoAgent.GetChosenAgent().idGun, shotThroughtCount, 3f);
                else if (distToEnemy <= 5.25f)
                    chanseToHit = calcChanse(infoAgent.GetChosenAgent().idGun, shotThroughtCount, 5.25f);
                else if (distToEnemy <= 7.6f)
                    chanseToHit = calcChanse(infoAgent.GetChosenAgent().idGun, shotThroughtCount, 7.6f);

                //Debug.LogWarning(distToEnemy);
                //Debug.LogWarning(Mathf.Round(distToEnemy));
                string chanseToHitT = chanseToHit.ToString() + " %";
                textInfo.transform.GetChild(1).GetComponent<Text>().text = chanseToHitT;
                textInfo.transform.GetChild(5).GetComponent<Text>().text = Mathf.Round(distToEnemy).ToString();
                textInfo.transform.GetChild(2).GetComponent<Text>().text = (35 - (shotThroughtCount * 5)).ToString();
                enemy1.chanseTohit = chanseToHit;
                enemy1.shotThroughtToEnemy = shotThroughtCount;

               // textInfo.transform.GetChild(textInfo.transform.childCount-1).gameObject.GetComponent<Button>().interactable = true;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    int random = Random.Range(0, 100);
                    if (enemy1.chanseTohit >= random)
                    {

                        int damage = 35 - (enemy1.shotThroughtToEnemy * 5);

                        enemy1.enemy.hp -= damage;
                        infoAgent.GetChosenAgent().isShooted = true;
                        infoAgent.SetStatusAgent(0);
                        manageQuads.ClearMap();
                        FieldUpdate();
                        print("Попал " + random);
                      //  textInfo.transform.GetChild(textInfo.transform.childCount - 1).gameObject.GetComponent<Button>().interactable = false;

                    }
                    else
                    {
                        infoAgent.GetChosenAgent().isShooted = true;
                        infoAgent.SetStatusAgent(0);
                        manageQuads.ClearMap();
                        FieldUpdate();
                        print("Не попал " + random);
                    }
                }
            }
            else if (lineHit.transform.gameObject.GetComponent<StatusEnemy>() != enemy1.enemy && 
                (lineHit.transform.gameObject.GetComponent<StatusEnemy>() || lineHit.transform.gameObject.GetComponent<StatusPlayer>()))
                 {
                     Vector3 hitedObjPos = lineHit.transform.gameObject.transform.position;
                     hitedObjPos.x = hitedPos.x;
                     hitedObjPos.y = hitedPos.y;

                     ShotCalculate(hitedObjPos, hitedPos, shotThroughtCount, distToEnemy);
                 }
                 else
                    return;
        }
    }

    // Альтернативный вариант стрельбы
    public void ShotButton()
    {

        // Обработка Shoot
        // Расчёт урона, процент шанса попадания
        if (infoAgent.isHaveChosenAgentBool())
        {
            int random = Random.Range(0, 100);
            if (enemy1.chanseTohit >= random)
            {

                int damage = 35 - (enemy1.shotThroughtToEnemy * 5);

                enemy1.enemy.hp -= damage;
                infoAgent.GetChosenAgent().isShooted = true;
                infoAgent.SetStatusAgent(0);
                manageQuads.ClearMap();
                FieldUpdate();
                print("Попал " + random);
                textInfo.transform.GetChild(textInfo.transform.childCount - 1).gameObject.GetComponent<Button>().interactable = false;

            }
            else
            {
                print("Не попал " + random);
            }
        }
        else
            print("Не выбран агент");

    }


    void SetChosenAgentPF(ref PathfindingNode node,GameObject agent)
    {
        if (start != null) start.isPlayer = false;
        node.target = infoAgent.FindPlayerOnNode(node.idNode).transform;
        start = node;
        node.isPlayer = true;
        node.cost = -1;
        node.mesh.material.color = pathColor;

        SetChoseAgent(agent);
    }

    ///		 Винтовка Дробовик Снайп.в.
    /// Ближ.  40		100		 25		|3f
    /// Сред.  100		50		 50		|5.25f
    /// Даль.  40		25		 100	|7.6f
    /// idGun  0        1        3

    public int calcChanse(int idGun, int shotThroughtC, float dist)
    {
        if (idGun == 0)
        {
            if (dist == 3f)
                return (int)(40 - (shotThroughtC * 10));
            else if (dist == 5.25f)
                return (int)(100 - (shotThroughtC * 10));
            else if (dist == 7.6f)
                return (int)(40 - (shotThroughtC * 10));

        }
        else if (idGun == 1)
        {
            if (dist == 3f)
                return (int)(100 - (shotThroughtC * 10));
            else if (dist == 5.25f)
                return (int)(50 - (shotThroughtC * 10));
            else if (dist == 7.6f)
                return (int)(25 - (shotThroughtC * 10));
        }
        else if (idGun == 2)
        {
            if (dist == 3f)
                return (int)(25 - (shotThroughtC * 10));
            else if (dist == 5.25f)
                return (int)(50 - (shotThroughtC * 10));
            else if (dist == 7.65f)
                return (int)(100 - (shotThroughtC * 10));
        }
        return 0;
    }

#if UNITY_EDITOR
    public PathfindingNode sample; // шаблон клетки
    public float sampleSize = 1; // размер клетки
    public void CreateGrid()
    {
        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i] != null) DestroyImmediate(grid[i].gameObject);
        }

        grid = new PathfindingNode[width * height];

        float posX = -sampleSize * width / 2f - sampleSize / 2f;
        float posY = sampleSize * height / 2f - sampleSize / 2f;
        float Xreset = posX;
        int z = 0;
        for (int y = 0; y < height; y++)
        {
            posY -= sampleSize;
            for (int x = 0; x < width; x++)
            {
                posX += sampleSize;
                PathfindingNode clone = Instantiate(sample, new Vector3(posX, 0, posY), Quaternion.identity, transform) as PathfindingNode;
                clone.transform.name = "Node-" + z;
                grid[z] = clone;
                z++;
            }
            posX = Xreset;
        }
    }
#endif
}
