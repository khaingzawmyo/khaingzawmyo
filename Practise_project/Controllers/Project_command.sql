Add-Migration InitialCreate
Update-Database

git pull origin developer --allow-unrelated-histories

--Git lab に切り替えるコメント
git remote set-url origin 【コピーしたGitLabのURL】
git push origin developer