using System;
using UnityEngine;

using Player    = SKKU.TEST_CODE_AFTER_2.Player;
using Enemy     = SKKU.TEST_CODE_AFTER_2.Enemy;

#region AFTER CODE
namespace SKKU.TEST_CODE_AFTER
{
    public class Character
    {
        protected int HP;           //체력
        protected int Damage;       //공격력
    
        /// <summary>
        /// 캐릭터 클래스 생성자
        /// </summary>
        public Character(int hp, int damage)
        {
            HP          = hp;
            Damage      = damage;
        }
    
        /// <summary>
        /// 공격을 받는 함수
        /// </summary>
        public virtual void TakeDamage()
        {
            Debug.Log("기본 캐릭터 데미지 공격 함수.");
        }
    }
    public class Player : Character
    {
        private int Stamina;

        public Player(int hp, int damage, int stamina) : base(hp, damage)
        {
            Stamina = stamina;
        }
        public override void TakeDamage()
        {
            Debug.Log("플레이어는 공격 들어오는 함수 다르다!");
        }
    }
    public class Enemy : Character
    {
        public Enemy(int hp, int damage) : base(hp, damage)
        {
        }
    }
}
#endregion

#region AFTER CODE 2
namespace SKKU.TEST_CODE_AFTER_2
{
    public class Character
    {
        public int HP;           //체력
        public int Damage;       //공격력

        public virtual void TakeDamage()
        {
            Debug.Log("(변경된 코드) 기본 캐릭터 데미지 공격 함수.");
        }
    }
    
    public class Player : Character
    {
        public int Stamina;
        
        public override void TakeDamage()
        {
            Debug.Log("(변경된 코드) 플레이어는 공격 들어오는 함수 다르다!");
        }
    }
    
    public class Enemy : Character
    {
        public override void TakeDamage()
        {
            base.TakeDamage();
            Debug.Log("(변경된 코드) 적 공격 들어오는 함수 다르다!");
        }
    }
    
}
#endregion

#region BEFORE CODE
namespace SKKU.TEST_CODE_BEFORE
{
    public class Player
    {
        public int HP;              //체력
        public int Damage;          //공격력
        public int Stamina;         //스태미나
        
        public void TakeDamage()
        {
            Debug.Log("(변경전 코드) 플레이어는 공격 들어오는 함수");
        }
    }

    public class Enemy
    {
        public int HP;              //체력
        public int Damage;          //공격력
        
        public void TakeDamage()
        {
            Debug.Log("(변경전 코드) 적들이 공격 들어오는 함수");
        }
    }

   
}
#endregion

public class TestCode : MonoBehaviour
{
    private void Start()
    {
        Player player   = new Player()  { HP = 100, Damage = 30, Stamina = 50 };
        Enemy enemy     = new Enemy()   { HP = 100, Damage = 30 };
        
        //플레이어 공격 받는 부분
        player.TakeDamage();
        
        //적 공격 받는 부분
        enemy.TakeDamage();
    }
}







