using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace YEONJI.Kindergarten
{
    /// <summary>
    /// 이동 실행, 애니메이션, GridMap 캡슐화(목적지 찾기, 경로 요청)하기
    /// </summary>
    public class KidAgent : MonoBehaviour
    {
        [SerializeField] private GameObject RequestBubble;
        [SerializeField] private GameObject wantPlayBubble;

        // 애니메이션 관련 변수
        private Animator animator;
        private float emotionTimer;

        // 이동 관련 변수
        private List<Vector2Int> path;
        private int index;
        private float percent;
        private float moveTime;
        private Vector3 segStart, segEnd;
        private float doorTimer;
        private float blockTimer;

        public bool IsMove { get; private set; }
        public Vector2Int CurrentCell { get; private set; }

        public Vector2Int NextCell => (IsMove && doorTimer <= 0f && path != null && index < path.Count)
            ? path[index]
            : CurrentCell;

        public KidsContext Context { get; private set; }

        private static readonly Vector2[] dirs =
        {
            Vector2.up, Vector2.right, Vector2.down, Vector2.left
        };

        private bool needBeginSegment;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            RequestBubble.SetActive(false);
            wantPlayBubble.SetActive(false);
            InGameCore.KIDS.Register(this);
            // 임시
            moveTime = 0.65f;
        }

        public void Init(Vector2Int spawnPos)
        {
            CurrentCell = spawnPos;
            animator.SetBool("isWalk", false);
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", -1f);
        }

        public void Moving(List<Vector2Int> newPath)
        {
            path = newPath;
            index = 1; // index 0 은 현재 위치이기 때문에 1부터 시작하는 것으로 해야 한다.
            IsMove = path != null && path.Count > 1;
            if (IsMove) BeginSegment();
        }

        public void BeginSegment()
        {
            segStart = transform.position;
            Vector2Int cell = path[index];
            segEnd = InGameCore.GRID.GridToWorld(cell.x, cell.y);
            Vector3 dir = segEnd - segStart;

            // 애니메이션 설정
            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
            animator.SetBool("isWalk", true);

            percent = 0f;
        }

        public void Tick(float dt)
        {
            if (!IsMove) return;

            if (emotionTimer > 0f)
            {
                emotionTimer -= dt;
                animator.SetBool("isWalk", false);
                return;
            }

            if (doorTimer > 0f)
            {
                doorTimer -= dt;
                animator.SetBool("isWalk", false);
                return;
            }

            if (needBeginSegment)
            {
                var occupied = InGameCore.KIDS.GetOccupied(this);
                if (occupied.Contains(path[index]))
                {
                    animator.SetBool("isWalk", false);
                    blockTimer += dt;

                    if (blockTimer > 1.5f)
                    {
                        IsMove = false;
                        blockTimer = 0f;
                    }

                    return;
                }

                BeginSegment();
                needBeginSegment = false;
                blockTimer = 0f;
            }

            percent += dt / moveTime;
            if (percent < 1)
            {
                transform.position = Vector3.Lerp(segStart, segEnd, percent);
                return;
            }

            transform.position = segEnd;
            CurrentCell = path[index];
            index++;

            if (index >= path.Count)
            {

                IsMove = false;
                animator.SetBool("isWalk", false);
            }
            else
            {
                Vector2Int nextCell = path[index];
                var curZone = InGameCore.GRID.GetCell(CurrentCell.x, CurrentCell.y).zone;
                var nextZone = InGameCore.GRID.GetCell(nextCell.x, nextCell.y).zone;

                if (curZone != nextZone)
                    doorTimer = 0.3f;
                needBeginSegment = true;
            }
        }

        public bool FindDestination(ZoneType zone, HashSet<Vector2Int> occupied, out Vector2Int des)
        {
            InGameCore.GRID.GetZoneBounds(zone, out var min, out var max);
            for (int i = 0; i < 10; i++) // 장애물이랑 겹쳐서 길을 못 찾을 시에는 10번 재시도
            {
                int x = Random.Range(min.x, max.x + 1);
                int y = Random.Range(min.y, max.y + 1);

                if (InGameCore.GRID.GetCell(x, y).zone != zone) continue;
                if (InGameCore.GRID.IsDoor(x, y)) continue;
                if (!InGameCore.GRID.IsWalkable(x, y)) continue;
                if (CurrentCell == new Vector2Int(x, y)) continue;
                if (occupied.Contains(new Vector2Int(x, y))) continue;

                des = new Vector2Int(x, y);
                return true;
            }

            des = default;
            return false;
        }

        public void RequestPath(Vector2Int des, HashSet<Vector2Int> occupied)
        {
            var newPath = PathFinder.FindPath(InGameCore.GRID, CurrentCell, des, occupied);
            if (newPath != null && newPath.Count > 1)
                Moving(newPath);
        }

        public void FaceRandomDir()
        {
            Vector2 dir = dirs[Random.Range(0, dirs.Length)];
            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
        }

        public void SetContext(KidsContext context)
        {
            Context = context;
        }

        public void GotCaught()
        {
            Context.machine.ChangeState(new WaitingState());
        }

        public void RequestWait()
        {
            if (!RequestBubble.activeSelf)
                RequestBubble.SetActive(true);
            animator.SetBool("isWalk", false);
            animator.SetBool("isTired", true);
            emotionTimer = 0.5f;
        }

        public void StopMove()
        {
            IsMove = false;
            path = null;
            animator.SetBool("isWalk", false);
        }

        public void RequestClear()
        {
            RequestBubble.SetActive(false);
            animator.SetBool("isTired", false);
            animator.SetBool("isAngry", false);
        }

        public void Angry()
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isAngry", true);
            animator.SetBool("isTired", false);
            emotionTimer = 0.5f;
        }

        public void showPlayBubble()
        {
            wantPlayBubble.SetActive(true);
        }

        public void finishGame()
        {
            wantPlayBubble.SetActive(false);
        }

    }
}