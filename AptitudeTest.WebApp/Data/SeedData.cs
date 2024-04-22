using AptitudeTest.WebApp.Models.Enum;
using AptitudeTest.WebApp.Models;

namespace AptitudeTest.WebApp.Data
{
    public class SeedData
    {
        public static void Seeding(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                //User Seeding
                if (!context.Users.Any())
                {
                    context.Users.Add(new User()
                    {
                        Email = "uyenvuhoang3112@gmail.com",
                        UserName = "admin",
                        Password = "admin123",
                        Role = (int)EnumRoles.MANAGER,
                        IsActive = true
                    });
                    context.SaveChanges();

                    context.Users.Add(new User()
                    {
                        Email = "uyenvuhoang.3112@gmail.com",
                        UserName = "uyenvuhoang",
                        Password = "uyen2003",
                        Role = (int)EnumRoles.CANDIDATE,
                        IsActive = false
                    });
                    context.SaveChanges();
                }

                //Exam Seeding
                if (!context.Exams.Any())
                {
                    context.Exams.AddRange(new List<Exam>()
                    {
                        new Exam()
                        {
                            Title = "General Knowledge",
                            Description = "Exam1",
                            Time = 20,
                            TakeRandomCount = 5
                        },
                        new Exam()
                        {
                            Title = "Mathematics",
                            Description = "Exam2",
                            Time = 20,
                            TakeRandomCount = 5
                        },
                        new Exam()
                        {
                            Title = "Computer Technology",
                            Description = "Exam3",
                            Time = 20,
                            TakeRandomCount = 5
                        }
                    });
                    context.SaveChanges();
                }

                //QnA Seeding
                if (!context.QnAs.Any())
                {
                    context.QnAs.AddRange(new List<QnA>()
                    {
                        //QnA for Exam 1
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "What is the primary component of natural gas ?",
                            Option1 = "Methane",
                            Option2 = "Ethane",
                            Option3 = "Propane",
                            Option4 = "Butane",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "Which country is the world's largest oil producer as of 2021 ?",
                            Option1 = "United State",
                            Option2 = "Russia",
                            Option3 = "Saudi Arabia",
                            Option4 = "China",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "What is the process called when crude oil is separated into different hydrocarbon components based on their boiling points ?",
                            Option1 = "Distillation",
                            Option2 = "Cracking",
                            Option3 = "Reforming",
                            Option4 = "Polymerization",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "What is the estimated age of the fossilized remains from which crude oil and natural gas are formed ?",
                            Option1 = "Milions of Years",
                            Option2 = "Thousands of Years",
                            Option3 = "Billions of Years",
                            Option4 = "Trillions of Years",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "Which organization is often referred to as the \"oil cartel\" and influences oil prices globally ?",
                            Option1 = "OPEC",
                            Option2 = "UN",
                            Option3 = "EU",
                            Option4 = "ASEAN",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "Which drilling technique involves drilling vertically and then horizontally to extract oil or gas from unconventional reservoirs ?",
                            Option1 = "Directional Drilling",
                            Option2 = "Offshore Drilling",
                            Option3 = "Hydraulic Fracturing (Fracking)",
                            Option4 = "Rotary Drilling",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "What is the process of using high-pressure fluids to create fractures in rock formations to release oil or gas ?",
                            Option1 = "Hydraulic Fracturing (Fracking)",
                            Option2 = "Seismic Exploration",
                            Option3 = "Geothermal Energy Extraction",
                            Option4 = "Oil Well Cementing",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "Which environmental concern is associated with offshore oil drilling ?",
                            Option1 = "Coral Reef Destruction",
                            Option2 = "Soil Erosion",
                            Option3 = "Air Pollution",
                            Option4 = "Groundwater Contamination",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "What is the term used for the process of converting heavy hydrocarbons into lighter ones to produce high-value products like gasoline ?",
                            Option1 = "Cracking",
                            Option2 = "Refining",
                            Option3 = "Fractionation",
                            Option4 = "Distillation",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 1,
                            Question = "Which country is known for having the largest proven oil reserves in the world as of 2021 ?",
                            Option1 = "Venezuela",
                            Option2 = "Saudi Arabia",
                            Option3 = "Canada",
                            Option4 = "Iraq",
                            TheAnswer = 1
                        },


                        //QnA for Exam 2
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "An Oil and Gas company has a total of 20 oil wells in operation. If they shut down 3 wells for maintenance, how many wells are still operational ?",
                            Option1 = "17 wells",
                            Option2 = "13 wells",
                            Option3 = "15 wells",
                            Option4 = "18 wells",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "The company's natural gas field has estimated reserves of 1.5 trillion cubic feet (TCF). If they produce 8 billion cubic feet of gas per day, how many days will their reserves last ?",
                            Option1 = "187.5 days",
                            Option2 = "186.5 days",
                            Option3 = "188.5 days",
                            Option4 = "189.5 days",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "The company's refinery processes 200,000 barrels of crude oil per day. If each barrel of crude oil yields 50% gasoline, how many barrels of gasoline are produced daily ?",
                            Option1 = "100,000 barrels",
                            Option2 = "98,000 barrels",
                            Option3 = "99,000 barrels",
                            Option4 = "101,000 barrels",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "An Oil and Gas company needs to transport 1.5 million barrels of oil. If each tanker truck can carry 10,000 barrels of oil, how many trucks are required for transportation ?",
                            Option1 = "150 trucks",
                            Option2 = "160 trucks",
                            Option3 = "170 trucks",
                            Option4 = "140 trucks",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "The company's oil tanker has a capacity of 2.8 million barrels. If they currently carry 2.5 million barrels, what percentage of the tanker's capacity is utilized ?",
                            Option1 = "89.3%",
                            Option2 = "88.5%",
                            Option3 = "87.3%",
                            Option4 = "90.5%",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "The company drills a new exploration well that reaches a depth of 4,500 meters. If each drilling pipe used is 30 meters long, how many pipes did they use in total for this well ?",
                            Option1 = "150 pipes",
                            Option2 = "160 pipes",
                            Option3 = "130 pipes",
                            Option4 = "140 pipes",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "An Oil and Gas company has three drilling rigs in operation. The first rig drills an average of 2 wells per month, the second rig drills 1.5 wells per month, and the third rig drills 2.5 wells per month. How many wells are drilled in total in 6 months ?",
                            Option1 = "15 wells",
                            Option2 = "13 wells",
                            Option3 = "14 wells",
                            Option4 = "16 wells",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "The company estimates that their natural gas production will increase by 25% in the next quarter. If they currently produce 4 million cubic feet of gas per day, how much gas will they produce in the next quarter ?",
                            Option1 = "5 million cubic feet",
                            Option2 = "3 million cubic feet",
                            Option3 = "4 million cubic feet",
                            Option4 = "6 million cubic feet",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "An Oil and Gas company spends 20% of its revenue on exploration, 40% on production costs, and 15% on transportation. If their total revenue is $800 million, how much money do they spend on production costs ?",
                            Option1 = "$320 million",
                            Option2 = "$430 million",
                            Option3 = "$540 million",
                            Option4 = "$330 million",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 2,
                            Question = "The company plans to invest $15 million in a new drilling project. If they secure 60% of the funding from investors, how much money do they need to raise ?",
                            Option1 = "$6 million",
                            Option2 = "$11 million",
                            Option3 = "$7 million",
                            Option4 = "$9 million",
                            TheAnswer = 1
                        },


                        //QnA for Exam 3
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "Which technology is used to track the movement of oil and gas products along the supply chain, ensuring transparency and traceability ?",
                            Option1 = "Blockchain",
                            Option2 = "Artificial Intelligence (AI)",
                            Option3 = "Virtual Reality (VR)",
                            Option4 = "Cloud Computing",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "Which computer-based system is used to manage and optimize the scheduling and routing of oil and gas shipments ?",
                            Option1 = "Enterprise Resource Planning",
                            Option2 = "Geographic Information System",
                            Option3 = "Internet of Things",
                            Option4 = "Augmented Readlity",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "What is the term used for the process of digitally mapping the subsurface rock formations to identify potential oil and gas reservoirs ?",
                            Option1 = "Seismic Imaging",
                            Option2 = "Reservoir Simulation",
                            Option3 = "3D Printing",
                            Option4 = "Data Mining",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "Which technology enables Oil and Gas companies to remotely monitor and control offshore drilling rigs and production facilities ?",
                            Option1 = "Drones",
                            Option2 = "Robotics",
                            Option3 = "Edge Computing",
                            Option4 = "Quantum Computing",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "What is the purpose of using SCADA (Supervisory Control and Data Acquisition) systems in Oil and Gas companies ?",
                            Option1 = "Real-time monitoring and control of industrial processes",
                            Option2 = "Exploration of new oil and gas reserves",
                            Option3 = "Managing financial transactions",
                            Option4 = "Designing marketing campaigns",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "Which programming language is widely used for data analysis, machine learning, and AI applications in the Oil and Gas industry ?",
                            Option1 = "Python",
                            Option2 = "C#",
                            Option3 = "JavaScript",
                            Option4 = "Swift",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "Which technology allows Oil and Gas companies to perform seismic surveys using autonomous underwater vehicles instead of traditional ships ?",
                            Option1 = "Robotics",
                            Option2 = "Augmented Reality (AR)",
                            Option3 = "LiDAR (Light Detection and Ranging)",
                            Option4 = "Internet of Things (IoT)",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "What is the term used for the computer-based modeling and analysis of the flow of fluids in pipelines and reservoirs ?",
                            Option1 = "Computational Fluid Dynamics (CFD)",
                            Option2 = "Geographic Information System (GIS)",
                            Option3 = "Cloud Computing",
                            Option4 = "Virtual Reality (VR)",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "Which technology is used for real-time monitoring of oil well parameters, allowing operators to optimize production and prevent accidents ?",
                            Option1 = "Internet of Things (IoT)",
                            Option2 = "Quantum Computing",
                            Option3 = "3D Printing",
                            Option4 = "Edge Computing",
                            TheAnswer = 1
                        },
                        new QnA()
                        {
                            ExamId = 3,
                            Question = "Which computer-based system is used to manage and analyze geological data for exploration and production planning ?",
                            Option1 = "Geographic Information System (GIS)",
                            Option2 = "Virtual Reality (VR)",
                            Option3 = "Enterprise Resource Planning (ERP)",
                            Option4 = "3D Printing",
                            TheAnswer = 1
                        },
                    });
                    context.SaveChanges();
                }
                
            }
        }
    }
}
