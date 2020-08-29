
for p in 1611 1612 1613 1614
do
	PORT=$p ./avx & disown
done
