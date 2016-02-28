using Amazon.EC2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiService.Deploy
{
    class Program
    {
        private static string ImageId = "ami-751fb006";
        private static string KeyPairName = "AmazonAMIKeyPair";
        private static string SimpleApiSecGroup = "sg-54234830";

        static void Main(string[] args)
        {
            
            var ec2ServiceClient = new AmazonEC2Client();

            // Create an EC2 creation request
            var newEc2InstanceRequest = new Amazon.EC2.Model.RunInstancesRequest()
            {
                ImageId = ImageId,
                InstanceType = InstanceType.T2Micro,
                MinCount = 1,
                MaxCount = 1,
                KeyName = KeyPairName,
                SecurityGroupIds = new List<string> { SimpleApiSecGroup },
                UserData = GetLaunchUserData(),
                IamInstanceProfile = new Amazon.EC2.Model.IamInstanceProfileSpecification()
                {
                    Name = "S3_Access"
                }
            };


            // Submit the request to AWS platform.
            var response = ec2ServiceClient.RunInstances(newEc2InstanceRequest);

            Console.WriteLine("EC2 creation request submitted Receipt='{0}'", response.Reservation.ReservationId);

            Console.ReadKey();
        }

        /// <summary>
        /// Bootstrap script that downloads and installs the application in the EC2 instance.
        /// </summary>
        /// <returns></returns>
        private static string GetLaunchUserData()
        {
            var userData = @"<powershell>
                            aws s3 cp s3://simpleapistartup/ c://deployment/ --recursive
                            cd c:\Deployment
                            .\ApiService.deploy.cmd /Y -enableRule:DoNotDelete
                            </powershell>";

            return Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(userData));
        }
    }
}
