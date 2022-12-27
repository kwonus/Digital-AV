set `echo $HOME | tr / ' '`
d=$1
u=$2
x=$3
echo home = $d:\\$u\\$x\\

mkdir ../FlatSharp
echo =======================

for i in *.fbs
do
    file=`echo $i | cut -d\. -f1`
    echo $file:

    # FlatSharp code-gen
    dotnet $d:\\$u\\$x\\'.nuget\packages\flatsharp.compiler\7.0.2\tools\net6.0\FlatSharp.Compiler.dll' --nullable-warnings true --normalize-field-names true --gen-poolable false --input $d':\src\Digital-AV\z-series\FB\IDL\'$i --output $d':\src\Digital-AV\z-series\FB\FlatSharp\'
    mv ../FlatSharp/FlatSharp.generated.cs ../FlatSharp/$file.cs

    # FlatBuffers code-gen
#   /$d/dev/flatbuffers/out/build/x64-Debug/flatc --csharp --natural-utf8 --force-empty -o '..\generated' $i
    /$d/dev/flatbuffers/out/build/x64-Debug/flatc --rust   --natural-utf8 --force-empty -o '..\examples\rust\generated' $i
done
