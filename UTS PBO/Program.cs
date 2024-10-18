using System;
using System.Collections.Generic;

namespace ArenaRobotTarung
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.PilihRobot();
            game.StartBattle();
        }
    }

    public class Game
    {
        Robot Pemain;
        Robot Musuh;
        List<Kemampuan> KemampuanPemain;
        List<Kemampuan> KemampuanMusuh;
        int turn = 1;

        public Game()
        {
            this.Musuh = new BossRobot("BossBot", 200, 50, 40);
            this.KemampuanMusuh = new List<Kemampuan>
            {
                new ElectricShock(),
                new PlasmaCannon(),
                new SuperShield()
            };
        }

        public void PilihRobot()
        {
            string choice;
            do
            {
                Console.WriteLine("Robot apakah kamu:");
                Console.WriteLine("1. Robot Electric");
                Console.WriteLine("2. Robot Plasma");
                Console.Write("Pilihan: ");
                choice = Console.ReadLine();
                Console.Clear();
            } while (choice != "1" && choice != "2");

            switch (choice)
            {
                case "1":
                    this.Pemain = new RobotElectric("Robot Electric", 100, 30, 25);
                    this.KemampuanPemain = new List<Kemampuan> { new ElectricShock(), new SuperShield() };
                    break;
                case "2":
                    this.Pemain = new RobotPlasma("Robot Plasma", 100, 35, 30);
                    this.KemampuanPemain = new List<Kemampuan> { new PlasmaCannon(), new SuperShield() };
                    break;
            }
        }

        public void StartBattle()
        {
            while (Pemain.energi > 0 && Musuh.energi > 0 && turn <= 10)
            {
                Console.Clear();
                Console.WriteLine("=====================================");
                Console.WriteLine($"               TURN {turn}             ");
                Console.WriteLine("=====================================");
                PlayerTurn();
                if (Musuh.energi <= 0) break;

                EnemyTurn();
                if (Pemain.energi <= 0) break;

                turn++;
            }

            Console.WriteLine("=====================================");
            Console.WriteLine("       !!Selamat PERTARUNGAN TELAH BERAKHIR!!");
            Console.WriteLine($"       Pemenangnya adalah: {(Pemain.energi > 0 ? Pemain.nama : Musuh.nama)}");
            Console.WriteLine("=====================================");
        }

        public void PlayerTurn()
        {
            Console.Clear();
            Console.WriteLine($"TURN {turn} - GILIRAN PEMAIN");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("1. Serang\n2. Gunakan Kemampuan");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Pemain.Serang(Musuh);
            }
            else if (choice == "2")
            {
                Console.WriteLine("Pilih kemampuan:");
                for (int i = 0; i < KemampuanPemain.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {KemampuanPemain[i].GetName()}");
                }
                int pilihankemampuan = int.Parse(Console.ReadLine()) - 1;
                Pemain.GunakanKemampuan(KemampuanPemain[pilihankemampuan], Musuh);
            }

            Console.WriteLine("\nInformasi Setelah Aksi:");
            Pemain.CetakInformasi();
            Musuh.CetakInformasi();

            // Menunggu input dari user sebelum melanjutkan
            Console.WriteLine("\nTekan Enter untuk lanjiyutz...");
            Console.ReadLine();
        }

        public void EnemyTurn()
        {
            Console.Clear();
            Console.WriteLine($"TURN {turn} - GILIRAN MUSUH");
            Console.WriteLine("-----------------------------");
            Random random = new Random();
            int action = random.Next(0, 2);

            if (action == 0)
            {
                Musuh.Serang(Pemain);
            }
            else
            {
                KemampuanMusuh[random.Next(KemampuanMusuh.Count)].Gunakan(Musuh, Pemain);
            }

            Console.WriteLine("\nInformasi Setelah Aksi:");
            Pemain.CetakInformasi();
            Musuh.CetakInformasi();

            // Menunggu input dari user sebelum melanjutkan
            Console.WriteLine("\nTekan Enter untuk lanjiyutz...");
            Console.ReadLine();
        }
    }

    // Kelas Abstrak Robot
    public abstract class Robot
    {
        public string nama;
        public int energi;
        public int armor;
        public int serangan;

        public Robot(string nama, int energi, int armor, int serangan)
        {
            this.nama = nama;
            this.energi = energi;
            this.armor = armor;
            this.serangan = serangan;
        }

        public void Serang(Robot target)
        {
            Console.WriteLine($"{nama} menyerang {target.nama}");
            int damage = serangan - target.armor;
            if (damage > 0)
            {
                // Jika target adalah BossRobot, panggil metode Diserang
                if (target is BossRobot boss)
                {
                    boss.Diserang(damage);
                }
                else
                {
                    target.energi -= damage;
                    Console.WriteLine($"{target.nama} menerima {damage} damage");
                }
            }
            else
            {
                Console.WriteLine($"{target.nama} berhasil memblok serangan!");
            }
        }

        public void GunakanKemampuan(Kemampuan kemampuan, Robot target)
        {
            kemampuan.Gunakan(this, target);
        }

        public abstract void CetakInformasi();
    }

    // Subclass Robot Electric
    public class RobotElectric : Robot
    {
        public RobotElectric(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan) { }

        public override void CetakInformasi()
        {
            Console.WriteLine($"[Robot Electric] Nama: {nama}, Energi: {energi}, Armor: {armor}, Serangan: {serangan}");
        }
    }

    // Subclass Robot Plasma
    public class RobotPlasma : Robot
    {
        public RobotPlasma(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan) { }

        public override void CetakInformasi()
        {
            Console.WriteLine($"[Robot Plasma] Nama: {nama}, Energi: {energi}, Armor: {armor}, Serangan: {serangan}");
        }
    }

    // Subclass BossRobot
    public class BossRobot : Robot
    {
        public BossRobot(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
        {
            // Set armor lebih besar untuk BossRobot
            this.armor += 20; // Contoh armor lebih besar dibanding robot biasa
        }

        // Metode untuk menangani serangan dari robot
        public void Diserang(int damage)
        {
            energi -= damage;
            Console.WriteLine($"{nama} diserang! Energi berkurang menjadi {energi}.");
            if (energi <= 0)
            {
                Mati();
            }
        }

        // Metode yang akan dipanggil jika energi habis
        public void Mati()
        {
            Console.WriteLine($"{nama} telah mati!");
        }

        public override void CetakInformasi()
        {
            Console.WriteLine($"[Boss Robot] Nama: {nama}, Energi: {energi}, Armor: {armor}, Serangan: {serangan}");
        }
    }

    // Interface Kemampuan
    public interface Kemampuan
    {
        void Gunakan(Robot pengguna, Robot target);
        string GetName();
    }

    // Kemampuan Repair
    public class Repair : Kemampuan
    {
        public void Gunakan(Robot pengguna, Robot target)
        {
            pengguna.energi += 20;
            Console.WriteLine($"{pengguna.nama} menggunakan Repair! Energi bertambah 20.");
        }

        public string GetName() => "Repair";
    }

    // Kemampuan ElectricShock
    public class ElectricShock : Kemampuan
    {
        public void Gunakan(Robot pengguna, Robot target)
        {
            target.energi -= 30;
            Console.WriteLine($"{pengguna.nama} menggunakan Electric Shock! {target.nama} kehilangan 30 energi.");
        }

        public string GetName() => "Electric Shock";
    }

    // Kemampuan PlasmaCannon
    public class PlasmaCannon : Kemampuan
    {
        public void Gunakan(Robot pengguna, Robot target)
        {
            target.energi -= 40;
            Console.WriteLine($"{pengguna.nama} menggunakan Plasma Cannon! {target.nama} kehilangan 40 energi.");
        }

        public string GetName() => "Plasma Cannon";
    }

    // Kemampuan SuperShield
    public class SuperShield : Kemampuan
    {
        public void Gunakan(Robot pengguna, Robot target)
        {
            pengguna.armor += 15;
            Console.WriteLine($"{pengguna.nama} menggunakan Super Shield! Armor bertambah 15.");
        }

        public string GetName() => "Super Shield";
    }
}