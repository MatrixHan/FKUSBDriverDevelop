//---------------------------------------------------------------------
// 资源打包器
//---------------------------------------------------------------------
using System.Windows.Forms;
using System.IO;
//---------------------------------------------------------------------
namespace FKResPacker
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("错误：没有打包资源参数...请拖拽需要打包的文件到本exe上");
                return 2;
            }

            // 生成的资源包文件路径
            string output_file = "FKResourcePack.resx";
            output_file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), output_file);

            // 逐个打到资源包里
            System.Resources.ResXResourceWriter rsxw = new System.Resources.ResXResourceWriter(output_file);
            for (int i = 0; i < args.Length; i++)
            {
                string strSourceResFile = args[i];
                if (File.Exists(strSourceResFile))
                {
                    FileStream fs = File.OpenRead(strSourceResFile);
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    fs.Close();

                    rsxw.AddResource(Path.GetFileName(strSourceResFile), data);
                }
                else
                {
                    MessageBox.Show(string.Format("错误：需要打包的资源文件 \"{0}\" 不存在...", strSourceResFile));
                    return 1;
                }
            }

            rsxw.Generate();
            rsxw.Close();

            MessageBox.Show(string.Format("打包完成，资源包目录为: \"{0}\"", output_file));

            return 0;
        }
    }
}
