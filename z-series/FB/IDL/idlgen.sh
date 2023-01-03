set `echo $HOME | tr / ' '`
d=$1
u=$2
x=$3
echo home = $d:\\$u\\$x\\

mkdir ../FlatSharp
mkdir ../zfblib
mkdir ../zfblib/src
echo =======================

for i in *.fbs
do
    file=`echo $i | cut -d\. -f1`
    echo -n $file.cs

    # FlatSharp code-gen
    dotnet $d:\\$u\\$x\\'.nuget\packages\flatsharp.compiler\7.0.2\tools\net6.0\FlatSharp.Compiler.dll' --nullable-warnings true --normalize-field-names true --gen-poolable false --input $d':\src\Digital-AV\z-series\FB\IDL\'$i --output $d':\src\Digital-AV\z-series\FB\FlatSharp\'
    mv ../FlatSharp/FlatSharp.generated.cs ../FlatSharp/$file.cs
    echo -n ,\ 

	rust=`echo $file | tr - _ | tr '[:upper:]' '[:lower:]'`
    echo -n $rust.rs
    # FlatBuffers code-gen
    /$d/dev/flatbuffers/out/build/x64-Debug/flatc --rust   --natural-utf8 --force-empty -o '..\zfblib\src\generated' $i
    mv ../zfblib/src/generated/${file}_generated.rs ../zfblib/src/generated/$rust.rs
	echo
done
